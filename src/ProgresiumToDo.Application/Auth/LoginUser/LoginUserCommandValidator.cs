using FluentValidation;

namespace ProgresiumToDo.Application.Auth.LoginUser;

internal sealed class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(luc => luc.Email).NotEmpty().EmailAddress(); // The existence of the email will be checked in the identity service
        
        RuleFor(luc => luc.Password).NotEmpty();
    }
}