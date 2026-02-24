namespace PromptProcessor.Domain;

public interface IPromptJobRepository
{
    Task<PromptJob> CreateAsync(PromptJob promptJob);
    Task<IEnumerable<PromptJob>> GetAllAsync();
    Task<PromptJob?> GetByIdAsync(Guid id);
    Task UpdateAsync(PromptJob job);
}