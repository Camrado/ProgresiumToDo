using FluentValidation;

namespace ProgresiumToDo.Application.Auth.Commands.VerifyEmail;

internal sealed class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator()
    {
        RuleFor(ve => ve.VerificationCode)
            .NotEmpty()
            .Length(6);
    }
}