using MediatR;
using PromptProcessor.Application.DTOs;

namespace PromptProcessor.Application.Commands.SubmitPrompt;

public record SubmitPromptCommand(string Prompt) : IRequest<PromptJobDto>;
