using MediatR;
using PromptProcessor.Application.DTOs;

namespace PromptProcessor.Application.Queries.GetPromptById;

public record GetPromptByIdQuery(Guid Id) : IRequest<PromptJobDto>;
