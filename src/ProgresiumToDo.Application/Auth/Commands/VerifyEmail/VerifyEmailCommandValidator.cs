using FluentValidation;

namespace ProgresiumToDo.Application.Auth.Commands.VerifyEmail;

internal sealed class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator()
    {
        RuleFor(ve => ve.VerificationCode)
            .NotEmpty()
            .Length(6)
            .Matches("^[0-9]{6}$");
    }
}