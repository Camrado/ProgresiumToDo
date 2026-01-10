using FluentValidation;

namespace ProgresiumToDo.Application.Users.Commands.UpdateProfile;

internal sealed class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(upc => upc.FirstName)
            .MaximumLength(100)
            .Must(firstName => string.IsNullOrWhiteSpace(firstName) || firstName.All(char.IsLetter))
            .WithMessage("First name must contain only letters.");
        
        RuleFor(upc => upc.LastName)
            .MaximumLength(100)
            .Must(lastName => string.IsNullOrWhiteSpace(lastName) || lastName.All(char.IsLetter))
            .WithMessage("First name must contain only letters.");
    }
}