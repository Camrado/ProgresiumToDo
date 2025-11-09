using FluentValidation;

namespace ProgresiumToDo.Application.Auth.VerifyEmail;

internal sealed class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator()
    {
        RuleFor(ve => ve.Token).NotEmpty();
    }
}