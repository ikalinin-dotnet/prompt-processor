namespace PromptProcessor.Application.DTOs;

public record PromptJobDto(
    Guid Id,
    string Prompt,
    string Status,
    string? Result,
    string? ErrorMessage,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
