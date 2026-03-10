using MassTransit;
using Microsoft.Extensions.Logging;
using PromptProcessor.Application.Services;
using PromptProcessor.Domain;
using PromptProcessor.Domain.Messages;

namespace PromptProcessor.Infrastructure.Consumers;

public class PromptJobConsumer : IConsumer<PromptJobCreated>
{
    private readonly IPromptJobRepository _repository;
    private readonly ILlmService _llmService;
    private readonly ILogger<PromptJobConsumer> _logger;

    public PromptJobConsumer(IPromptJobRepository repository, ILlmService llmService,
        ILogger<PromptJobConsumer> logger)
    {
        _repository = repository;
        _llmService = llmService;
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

        job.MarkAsProcessing();
        await _repository.UpdateAsync(job);

        try
        {
            var result = await _llmService.GetCompletionAsync(job.Prompt, context.CancellationToken);
            job.Complete(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process job {JobId}", jobId);
            job.Fail(ex.Message);
        }

        await _repository.UpdateAsync(job);
    }
}
