using FluentAssertions;
using PromptProcessor.Domain;
using PromptProcessor.Domain.Exceptions;

namespace PromptProcessor.Tests.Domain;

public class PromptJobTests
{
    // --- Create ---

    [Fact]
    public void Create_SetsPromptAndPendingStatus()
    {
        var job = PromptJob.Create("Hello world");

        job.Prompt.Should().Be("Hello world");
        job.Status.Should().Be(PromptStatus.Pending);
        job.Id.Should().NotBeEmpty();
        job.Result.Should().BeNull();
        job.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void Create_SetsCreatedAtAndUpdatedAt()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var job = PromptJob.Create("test");
        var after = DateTime.UtcNow.AddSeconds(1);

        job.CreatedAt.Should().BeAfter(before).And.BeBefore(after);
        job.UpdatedAt.Should().BeAfter(before).And.BeBefore(after);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_ThrowsForBlankPrompt(string? prompt)
    {
        var act = () => PromptJob.Create(prompt!);
        act.Should().Throw<ArgumentException>();
    }

    // --- MarkAsProcessing ---

    [Fact]
    public void MarkAsProcessing_FromPending_SetsProcessingStatus()
    {
        var job = PromptJob.Create("test");
        job.MarkAsProcessing();
        job.Status.Should().Be(PromptStatus.Processing);
    }

    [Fact]
    public void MarkAsProcessing_FromCompleted_ThrowsDomainException()
    {
        var job = PromptJob.Create("test");
        job.MarkAsProcessing();
        job.Complete("result");

        var act = () => job.MarkAsProcessing();
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void MarkAsProcessing_FromFailed_ThrowsDomainException()
    {
        var job = PromptJob.Create("test");
        job.MarkAsProcessing();
        job.Fail("err");

        var act = () => job.MarkAsProcessing();
        act.Should().Throw<DomainException>();
    }

    // --- Complete ---

    [Fact]
    public void Complete_FromProcessing_SetsCompletedAndResult()
    {
        var job = PromptJob.Create("test");
        job.MarkAsProcessing();
        job.Complete("the answer");

        job.Status.Should().Be(PromptStatus.Completed);
        job.Result.Should().Be("the answer");
    }

    [Fact]
    public void Complete_FromPending_ThrowsDomainException()
    {
        var job = PromptJob.Create("test");
        var act = () => job.Complete("result");
        act.Should().Throw<DomainException>();
    }

    // --- Fail ---

    [Fact]
    public void Fail_FromProcessing_SetsFailedAndErrorMessage()
    {
        var job = PromptJob.Create("test");
        job.MarkAsProcessing();
        job.Fail("something broke");

        job.Status.Should().Be(PromptStatus.Failed);
        job.ErrorMessage.Should().Be("something broke");
    }

    [Fact]
    public void Fail_FromPending_ThrowsDomainException()
    {
        var job = PromptJob.Create("test");
        var act = () => job.Fail("error");
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Fail_FromCompleted_ThrowsDomainException()
    {
        var job = PromptJob.Create("test");
        job.MarkAsProcessing();
        job.Complete("result");

        var act = () => job.Fail("error");
        act.Should().Throw<DomainException>();
    }
}
