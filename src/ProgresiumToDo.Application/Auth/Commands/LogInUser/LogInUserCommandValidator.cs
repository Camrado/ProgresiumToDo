using FluentValidation;

namespace ProgresiumToDo.Application.Auth.Commands.LogInUser;

internal sealed class LogInUserCommandValidator : AbstractValidator<LogInUserCommand>
{
    public LogInUserCommandValidator()
    {
        RuleFor(luc => luc.Email).NotEmpty().EmailAddress(); // The existence of the email will be checked in the identity service
        
        RuleFor(luc => luc.Password).NotEmpty();
    }
}