using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PromptProcessor.Domain;
using PromptProcessor.Domain.Messages;

namespace PromptProcessor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PromptsController : ControllerBase
{
    private readonly IPromptJobRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;

    public PromptsController(IPromptJobRepository repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePromptRequest request)
    {
        var job = new PromptJob()
        {
            Id = Guid.NewGuid(),
            Prompt = request.Prompt,
            Status = PromptStatus.Pending,
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
            UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
        };
        
        await _repository.CreateAsync(job);
        await _publishEndpoint.Publish(new PromptJobCreated(job.Id));
        
        return CreatedAtAction(nameof(GetById), new { id = job.Id }, job);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var jobs = await _repository.GetAllAsync();
        return Ok(jobs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var job = await _repository.GetByIdAsync(id);

        if (job is null)
        {
            return NotFound();
        }
        
        return Ok(job);
    }
}

public record CreatePromptRequest(string Prompt);