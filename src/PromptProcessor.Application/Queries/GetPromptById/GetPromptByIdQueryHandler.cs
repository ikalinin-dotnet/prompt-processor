using MediatR;
using PromptProcessor.Application.DTOs;
using PromptProcessor.Domain;
using PromptProcessor.Domain.Exceptions;

namespace PromptProcessor.Application.Queries.GetPromptById;

public class GetPromptByIdQueryHandler : IRequestHandler<GetPromptByIdQuery, PromptJobDto>
{
    private readonly IPromptJobRepository _repository;

    public GetPromptByIdQueryHandler(IPromptJobRepository repository)
    {
        _repository = repository;
    }

    public async Task<PromptJobDto> Handle(GetPromptByIdQuery request, CancellationToken cancellationToken)
    {
        var job = await _repository.GetByIdAsync(request.Id);

        if (job is null)
            throw new NotFoundException($"Prompt job '{request.Id}' was not found.");

        return job.ToDto();
    }
}
