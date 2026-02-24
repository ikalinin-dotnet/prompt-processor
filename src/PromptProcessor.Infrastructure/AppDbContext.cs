using Microsoft.EntityFrameworkCore;
using PromptProcessor.Domain;

namespace PromptProcessor.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<PromptJob> PromptJobs => Set<PromptJob>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PromptJob>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Prompt).IsRequired();
            entity.Property(e => e.Status)
                .HasConversion<string>();
        });
    }
}