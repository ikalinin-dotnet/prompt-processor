using Microsoft.EntityFrameworkCore;
using PromptProcessor.Domain;
using PromptProcessor.Infrastructure.Data;

namespace PromptProcessor.Infrastructure.Repositories;

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
        return await _context.PromptJobs
            .AsNoTracking()
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
    }

    public async Task<PromptJob?> GetByIdAsync(Guid id)
    {
        return await _context.PromptJobs.FindAsync(id);
    }

    public async Task UpdateAsync(PromptJob job)
    {
        ArgumentNullException.ThrowIfNull(job);
        _context.PromptJobs.Update(job);
        await _context.SaveChangesAsync();
    }
}
