namespace PromptProcessor.Application.Services;

public interface ILlmService
{
    Task<string> GetCompletionAsync(string prompt, CancellationToken cancellationToken = default);
}
