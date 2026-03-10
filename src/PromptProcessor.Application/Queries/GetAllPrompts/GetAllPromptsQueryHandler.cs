using MediatR;
using PromptProcessor.Application.DTOs;
using PromptProcessor.Domain;

namespace PromptProcessor.Application.Queries.GetAllPrompts;

public class GetAllPromptsQueryHandler : IRequestHandler<GetAllPromptsQuery, IEnumerable<PromptJobDto>>
{
    private readonly IPromptJobRepository _repository;

    public GetAllPromptsQueryHandler(IPromptJobRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PromptJobDto>> Handle(GetAllPromptsQuery request, CancellationToken cancellationToken)
    {
        var jobs = await _repository.GetAllAsync();
        return jobs.Select(j => j.ToDto());
    }
}
