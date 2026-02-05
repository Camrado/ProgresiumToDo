using FluentValidation;

namespace ProgresiumToDo.Application.Support.Commands.ContactUs;

internal sealed class ContactUsCommandValidator : AbstractValidator<ContactUsCommand>
{
    public ContactUsCommandValidator()
    {
        RuleFor(cuc => cuc.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.");

        RuleFor(cuc => cuc.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("A valid email is required.");

        RuleFor(cuc => cuc.Message)
            .NotEmpty()
            .WithMessage("Message is required.")
            .MaximumLength(1000)
            .WithMessage("Message must not exceed 1000 characters.");
        
        RuleFor(cuc => cuc.Subject)
            .NotEmpty()
            .WithMessage("Subject is required.")
            .MaximumLength(200)
            .WithMessage("Subject must not exceed 200 characters.");
    }
}