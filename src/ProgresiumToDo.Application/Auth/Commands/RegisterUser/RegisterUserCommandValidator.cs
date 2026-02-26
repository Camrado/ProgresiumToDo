using FluentValidation;
using ProgresiumToDo.Application.Users.Repositories;

namespace ProgresiumToDo.Application.Auth.Commands.RegisterUser;

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
                var existingUser = await userRepository.GetByEmailAsync(email, cancellationToken: cancellationToken);
                return existingUser == null;
            })
            .WithMessage("An account with this email already exists.");
        
        RuleFor(ruc => ruc.Password)
            .NotEmpty()
            .WithMessage("New password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one number.");
    }
}