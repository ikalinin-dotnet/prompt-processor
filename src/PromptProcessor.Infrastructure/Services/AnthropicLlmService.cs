using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;
using PromptProcessor.Application.Services;

namespace PromptProcessor.Infrastructure.Services;

public class AnthropicLlmService : ILlmService
{
    private readonly AnthropicClient _client;

    public AnthropicLlmService(AnthropicClient client)
    {
        _client = client;
    }

    public async Task<string> GetCompletionAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var response = await _client.Messages.GetClaudeMessageAsync(
            new MessageParameters
            {
                Model = AnthropicModels.Claude46Sonnet,
                MaxTokens = 1024,
                Messages = [new Message(RoleType.User, prompt)]
            }, cancellationToken);

        return response.Content.OfType<TextContent>().FirstOrDefault()?.Text ?? string.Empty;
    }
}
