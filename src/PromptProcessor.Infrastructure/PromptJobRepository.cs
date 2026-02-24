using Microsoft.EntityFrameworkCore;
using PromptProcessor.Domain;

namespace PromptProcessor.Infrastructure;

public class PromptJobRepository : IPromptJobRepository
{
    private readonly AppDbContext _context;

    public PromptJobRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<PromptJob> CreateAsync(PromptJob job)
    {
        ArgumentNullException.ThrowIfNull(job);
        _context.PromptJobs.Add(job);
        await _context.SaveChangesAsync();
        return job;
    }

    public async Task<IEnumerable<PromptJob>> GetAllAsync()
    {
        return await _context.PromptJobs.OrderByDescending(j => j.CreatedAt).ToListAsync();
    }

    public async Task<PromptJob?> GetByIdAsync(Guid id)
    {
        return await _context.PromptJobs.FindAsync(id);
    }

    public async Task UpdateAsync(PromptJob job)
    {
        ArgumentNullException.ThrowIfNull(job);
        job.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
        _context.PromptJobs.Update(job);
        await _context.SaveChangesAsync();
    }
}