using FluentValidation;

namespace PromptProcessor.Application.Commands.SubmitPrompt;

public class SubmitPromptCommandValidator : AbstractValidator<SubmitPromptCommand>
{
    public SubmitPromptCommandValidator()
    {
        RuleFor(x => x.Prompt)
            .NotEmpty().WithMessage("Prompt must not be empty.")
            .MaximumLength(4000).WithMessage("Prompt must not exceed 4000 characters.");
    }
}
