using FluentAssertions;
using MassTransit;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using PromptProcessor.Application.Services;
using PromptProcessor.Domain;
using PromptProcessor.Domain.Messages;
using PromptProcessor.Infrastructure.Consumers;
using Microsoft.Extensions.Logging;

namespace PromptProcessor.Tests.Application;

public class PromptJobConsumerTests
{
    private readonly IPromptJobRepository _repository = Substitute.For<IPromptJobRepository>();
    private readonly ILlmService _llmService = Substitute.For<ILlmService>();
    private readonly ILogger<PromptJobConsumer> _logger = Substitute.For<ILogger<PromptJobConsumer>>();
    private readonly PromptJobConsumer _consumer;

    public PromptJobConsumerTests()
    {
        _consumer = new PromptJobConsumer(_repository, _llmService, _logger);
    }

    private static ConsumeContext<PromptJobCreated> MakeContext(Guid jobId)
    {
        var ctx = Substitute.For<ConsumeContext<PromptJobCreated>>();
        ctx.Message.Returns(new PromptJobCreated(jobId));
        ctx.CancellationToken.Returns(CancellationToken.None);
        return ctx;
    }

    [Fact]
    public async Task Consume_CompletesJob_WhenLlmSucceeds()
    {
        var job = PromptJob.Create("What is 2+2?");
        _repository.GetByIdAsync(job.Id).Returns(job);
        _llmService.GetCompletionAsync(job.Prompt, Arg.Any<CancellationToken>()).Returns("4");

        await _consumer.Consume(MakeContext(job.Id));

        job.Status.Should().Be(PromptStatus.Completed);
        job.Result.Should().Be("4");
        await _repository.Received(2).UpdateAsync(job);
    }

    [Fact]
    public async Task Consume_FailsJob_WhenLlmThrows()
    {
        var job = PromptJob.Create("bad prompt");
        _repository.GetByIdAsync(job.Id).Returns(job);
        _llmService.GetCompletionAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("API error"));

        await _consumer.Consume(MakeContext(job.Id));

        job.Status.Should().Be(PromptStatus.Failed);
        job.ErrorMessage.Should().Be("API error");
    }

    [Fact]
    public async Task Consume_DoesNothing_WhenJobNotFound()
    {
        var missingId = Guid.NewGuid();
        _repository.GetByIdAsync(missingId).Returns((PromptJob?)null);

        await _consumer.Consume(MakeContext(missingId));

        await _llmService.DidNotReceive().GetCompletionAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<PromptJob>());
    }
}
