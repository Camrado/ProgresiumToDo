using FluentValidation;

namespace ProgresiumToDo.Application.Auth.Commands.SendForgotPasswordEmail;

internal sealed class SendForgotPasswordEmailCommandValidator : AbstractValidator<SendForgotPasswordEmailCommand>
{
    public SendForgotPasswordEmailCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email address is not valid");
    }
}
