using MassTransit;
using MediatR;
using PromptProcessor.Application.DTOs;
using PromptProcessor.Domain;
using PromptProcessor.Domain.Messages;

namespace PromptProcessor.Application.Commands.SubmitPrompt;

public class SubmitPromptCommandHandler : IRequestHandler<SubmitPromptCommand, PromptJobDto>
{
    private readonly IPromptJobRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;

    public SubmitPromptCommandHandler(IPromptJobRepository repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<PromptJobDto> Handle(SubmitPromptCommand request, CancellationToken cancellationToken)
    {
        var job = PromptJob.Create(request.Prompt);
        await _repository.CreateAsync(job);
        await _publishEndpoint.Publish(new PromptJobCreated(job.Id), cancellationToken);
        return job.ToDto();
    }
}
