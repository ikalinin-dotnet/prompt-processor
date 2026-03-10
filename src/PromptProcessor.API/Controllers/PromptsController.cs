using MediatR;
using Microsoft.AspNetCore.Mvc;
using PromptProcessor.Application.Commands.SubmitPrompt;
using PromptProcessor.Application.Queries.GetAllPrompts;
using PromptProcessor.Application.Queries.GetPromptById;

namespace PromptProcessor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PromptsController : ControllerBase
{
    private readonly ISender _sender;

    public PromptsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePromptRequest request)
    {
        var dto = await _sender.Send(new SubmitPromptCommand(request.Prompt));
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var dtos = await _sender.Send(new GetAllPromptsQuery());
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var dto = await _sender.Send(new GetPromptByIdQuery(id));
        return Ok(dto);
    }
}

public record CreatePromptRequest(string Prompt);
