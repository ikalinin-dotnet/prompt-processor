using MediatR;
using PromptProcessor.Application.DTOs;

namespace PromptProcessor.Application.Queries.GetAllPrompts;

public record GetAllPromptsQuery : IRequest<IEnumerable<PromptJobDto>>;
