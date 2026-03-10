using PromptProcessor.Domain;

namespace PromptProcessor.Application.DTOs;

internal static class PromptJobMappingExtensions
{
    internal static PromptJobDto ToDto(this PromptJob job) =>
        new(job.Id, job.Prompt, job.Status.ToString(), job.Result, job.ErrorMessage, job.CreatedAt, job.UpdatedAt);
}
