namespace PromptProcessor.Domain;

public class PromptJob
{
    public Guid Id { get; set; }
    public string Prompt { get; set; } = string.Empty;
    public PromptStatus Status { get; set; }
    public string? Result { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public enum PromptStatus
{
    Pending,
    Processing,
    Completed,
    Failed
}