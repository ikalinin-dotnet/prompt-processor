using FluentAssertions;
using MassTransit;
using NSubstitute;
using PromptProcessor.Application.Commands.SubmitPrompt;
using PromptProcessor.Domain;
using PromptProcessor.Domain.Messages;

namespace PromptProcessor.Tests.Application;

public class SubmitPromptCommandHandlerTests
{
    private readonly IPromptJobRepository _repository = Substitute.For<IPromptJobRepository>();
    private readonly IPublishEndpoint _publishEndpoint = Substitute.For<IPublishEndpoint>();
    private readonly SubmitPromptCommandHandler _handler;

    public SubmitPromptCommandHandlerTests()
    {
        _repository.CreateAsync(Arg.Any<PromptJob>())
            .Returns(callInfo => callInfo.Arg<PromptJob>());

        _handler = new SubmitPromptCommandHandler(_repository, _publishEndpoint);
    }

    [Fact]
    public async Task Handle_CreatesJobWithPendingStatus()
    {
        var command = new SubmitPromptCommand("Tell me a joke");

        var dto = await _handler.Handle(command, CancellationToken.None);

        dto.Prompt.Should().Be("Tell me a joke");
        dto.Status.Should().Be("Pending");
        dto.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_PersistsJobToRepository()
    {
        var command = new SubmitPromptCommand("test prompt");

        await _handler.Handle(command, CancellationToken.None);

        await _repository.Received(1).CreateAsync(Arg.Is<PromptJob>(j =>
            j.Prompt == "test prompt" && j.Status == PromptStatus.Pending));
    }

    [Fact]
    public async Task Handle_PublishesPromptJobCreatedMessage()
    {
        var command = new SubmitPromptCommand("test prompt");

        var dto = await _handler.Handle(command, CancellationToken.None);

        await _publishEndpoint.Received(1).Publish(
            Arg.Is<PromptJobCreated>(m => m.JobId == dto.Id),
            Arg.Any<CancellationToken>());
    }
}
