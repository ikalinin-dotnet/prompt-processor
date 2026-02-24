using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;
using MassTransit;
using Microsoft.Extensions.Logging;
using PromptProcessor.Domain;
using PromptProcessor.Domain.Messages;

namespace PromptProcessor.Infrastructure.Consumers;

public class PromptJobConsumer : IConsumer<PromptJobCreated>
{
    private readonly IPromptJobRepository _repository;
    private readonly AnthropicClient _anthropicClient;
    private readonly ILogger<PromptJobConsumer> _logger;

    public PromptJobConsumer(IPromptJobRepository repository, AnthropicClient anthropicClient,
        ILogger<PromptJobConsumer> logger)
    {
        _repository = repository;
        _anthropicClient = anthropicClient;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<PromptJobCreated> context)
    {
        var jobId = context.Message.JobId;
        var job = await _repository.GetByIdAsync(jobId);

        if (job is null)
        {
            _logger.LogWarning("Job {JobId} was not found", jobId);
            return;
        }

        job.Status = PromptStatus.Processing;
        await _repository.UpdateAsync(job);

        try
        {
            var response = await _anthropicClient.Messages.GetClaudeMessageAsync(
                new MessageParameters
                {
                    Model = AnthropicModels.Claude46Sonnet,
                    MaxTokens = 1024,
                    Messages    = [new Message(RoleType.User, job.Prompt)]
                });
            
            job.Status = PromptStatus.Completed;
            job.Result = response.Content.OfType<TextContent>().FirstOrDefault()?.Text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process job {JobId}", jobId);
            job.Status = PromptStatus.Failed;
            job.ErrorMessage = ex.Message;
        }
        
        await _repository.UpdateAsync(job);
    }
    
}