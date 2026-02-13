using FluentValidation;
using ProgresiumToDo.Application.Tags.Repositories;

namespace ProgresiumToDo.Application.Tags.Commands.CreateTag;

internal sealed class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator(ITagRepository tagRepository)
    {   
        RuleFor(ctc => ctc.Name)
            .NotEmpty()
            .WithMessage("Tag name is required.")
            .MaximumLength(255)
            .WithMessage("Tag name must not exceed 255 characters.")
            .MustAsync(async (command, name, cancellationToken) =>
            {
                var existingTag = await tagRepository.GetByNameAsync(name, cancellationToken);
                return existingTag is null;
            })
            .WithMessage("A tag with the same name already exists.");
    }
}