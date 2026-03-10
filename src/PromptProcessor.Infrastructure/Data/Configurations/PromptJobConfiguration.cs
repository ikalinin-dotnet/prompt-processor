using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromptProcessor.Domain;

namespace PromptProcessor.Infrastructure.Data.Configurations;

public class PromptJobConfiguration : IEntityTypeConfiguration<PromptJob>
{
    public void Configure(EntityTypeBuilder<PromptJob> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Prompt).IsRequired();
        builder.Property(e => e.Status).HasConversion<string>();
    }
}
