using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Users.Repositories;

namespace ProgresiumToDo.Application.Users.Commands.UpdateProfile;

internal sealed class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator(IUserContext userContext, IUserRepository userRepository)
    {
        RuleFor(upc => upc.FirstName)
            .MaximumLength(100)
            .Must(firstName => string.IsNullOrWhiteSpace(firstName) || firstName.All(char.IsLetter))
            .WithMessage("First name must contain only letters.");
        
        RuleFor(upc => upc.LastName)
            .MaximumLength(100)
            .Must(lastName => string.IsNullOrWhiteSpace(lastName) || lastName.All(char.IsLetter))
            .WithMessage("First name must contain only letters.");

        When(upc => !string.IsNullOrEmpty(upc.Email), () =>
        {
            RuleFor(upc => upc.Email)
                .Cascade(CascadeMode.Stop)
                .Must(email => string.IsNullOrEmpty(email) || !userContext.IsEmailVerified)
                .WithMessage("Email address is already verified and cannot be changed.")
                .EmailAddress()
                .WithMessage("A valid email address is required.")
                .MustAsync(async (command, email, cancellationToken) =>
                {
                    var user = await userRepository.GetByIdAsync(userContext.UserId, cancellationToken: cancellationToken);
                    if (user is null)
                        return false;
                    
                    // Using user.Email instead of userContext.Email to ensure we are comparing against
                    // the current email in the database, not just the one in the context which might be outdated.
                    return !email.Equals(user.Email, StringComparison.OrdinalIgnoreCase);
                })
                .WithMessage("The new email address must be different from the current one.")
                .MustAsync(async (command, email, cancellationToken) =>
                {
                    var existingUser = await userRepository.GetByEmailAsync(email, cancellationToken: cancellationToken);
                    return existingUser is null;
                })
                .WithMessage("The email address is already in use.");
        });
    }
}