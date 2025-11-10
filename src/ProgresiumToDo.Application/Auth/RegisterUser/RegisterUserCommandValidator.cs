using FluentValidation;
using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Application.Auth.RegisterUser;

internal sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator(IUserRepository userRepository)
    {
        RuleFor(ruc => ruc.FirstName).NotEmpty();

        RuleFor(ruc => ruc.LastName).NotEmpty();

        RuleFor(ruc => ruc.Email)
            .NotEmpty()
            .EmailAddress()
            .MustAsync(async (email, cancellationToken) =>
            {
                var existingUser = await userRepository.GetByEmailAsync(email, cancellationToken);
                return existingUser == null;
            })
            .WithMessage("An account with this email already exists.");
        
        RuleFor(ruc => ruc.Password).NotEmpty(); // The password complexity check is done in the identity service
    }
}