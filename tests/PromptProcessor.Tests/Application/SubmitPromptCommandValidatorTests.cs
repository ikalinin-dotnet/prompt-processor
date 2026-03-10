using FluentAssertions;
using PromptProcessor.Application.Commands.SubmitPrompt;

namespace PromptProcessor.Tests.Application;

public class SubmitPromptCommandValidatorTests
{
    private readonly SubmitPromptCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidPrompt_Passes()
    {
        var result = _validator.Validate(new SubmitPromptCommand("What is the meaning of life?"));
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_EmptyPrompt_Fails(string prompt)
    {
        var result = _validator.Validate(new SubmitPromptCommand(prompt));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Prompt");
    }

    [Fact]
    public void Validate_PromptExceeds4000Chars_Fails()
    {
        var longPrompt = new string('a', 4001);
        var result = _validator.Validate(new SubmitPromptCommand(longPrompt));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Prompt");
    }

    [Fact]
    public void Validate_PromptAtMaxLength_Passes()
    {
        var prompt = new string('a', 4000);
        var result = _validator.Validate(new SubmitPromptCommand(prompt));
        result.IsValid.Should().BeTrue();
    }
}
