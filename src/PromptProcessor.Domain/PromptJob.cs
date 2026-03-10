using PromptProcessor.Domain.Exceptions;

namespace PromptProcessor.Domain;

public class PromptJob
{
    public Guid Id { get; private set; }
    public string Prompt { get; private set; } = string.Empty;
    public PromptStatus Status { get; private set; }
    public string? Result { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Required by EF Core
    private PromptJob() { }

    public static PromptJob Create(string prompt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);

        return new PromptJob
        {
            Id = Guid.NewGuid(),
            Prompt = prompt,
            Status = PromptStatus.Pending,
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
            UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
        };
    }

    public void MarkAsProcessing()
    {
        if (Status != PromptStatus.Pending)
            throw new DomainException($"Cannot mark job as Processing from status '{Status}'.");

        Status = PromptStatus.Processing;
        UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
    }

    public void Complete(string result)
    {
        if (Status != PromptStatus.Processing)
            throw new DomainException($"Cannot complete job from status '{Status}'.");

        Status = PromptStatus.Completed;
        Result = result;
        UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
    }

    public void Fail(string errorMessage)
    {
        if (Status != PromptStatus.Processing)
            throw new DomainException($"Cannot fail job from status '{Status}'.");

        Status = PromptStatus.Failed;
        ErrorMessage = errorMessage;
        UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
    }
}

public enum PromptStatus
{
    Pending,
    Processing,
    Completed,
    Failed
}
