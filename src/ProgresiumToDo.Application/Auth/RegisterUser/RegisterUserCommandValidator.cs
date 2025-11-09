using FluentValidation;

namespace ProgresiumToDo.Application.Auth.RegisterUser;

internal sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(ruc => ruc.FirstName).NotEmpty();

        RuleFor(ruc => ruc.LastName).NotEmpty();

        RuleFor(ruc => ruc.Email).NotEmpty().EmailAddress(); // The uniqueness check is done in the identity service
        
        RuleFor(ruc => ruc.Password).NotEmpty(); // The password complexity check is done in the identity service
    }
}