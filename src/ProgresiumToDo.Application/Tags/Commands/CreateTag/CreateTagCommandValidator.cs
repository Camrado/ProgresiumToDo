using FluentValidation;
using ProgresiumToDo.Application.Tags.Repositories;

namespace ProgresiumToDo.Application.Tags.Commands.CreateTag;

internal sealed class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator(ITagRepository tagRepository)
    {
        RuleFor(ctc => ctc)
            .MustAsync(async (command, cancellationToken) =>
            {
                var existingTag = await tagRepository.GetByNameAsync(command.Name, cancellationToken);
                return existingTag is null;
            })
            .WithMessage("A tag with the same name already exists.");
        
        RuleFor(ctc => ctc.Name)
            .NotEmpty()
            .WithMessage("Tag name is required.")
            .MaximumLength(255)
            .WithMessage("Tag name must not exceed 255 characters.");
        
        RuleFor(ctc => ctc.Color)
            .NotEmpty()
            .WithMessage("Color is required.")
            .Matches("^#[0-9A-Fa-f]{6}$")
            .WithMessage("Color must be a valid hex value like #000000");
    }
}