using FluentValidation;
using ProgresiumToDo.Application.Tags.Repositories;

namespace ProgresiumToDo.Application.Tags.Commands.UpdateTag;

internal sealed class UpdateTagCommandValidator : AbstractValidator<UpdateTagCommand>
{
    public UpdateTagCommandValidator(ITagRepository tagRepository)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        
        RuleFor(utc => utc.TagId)
            .NotEmpty()
            .WithMessage("TagId must not be empty.")
            .MustAsync(async (command, tagId, cancellationToken) =>
            {
                var tag = await tagRepository.GetByIdAsync(tagId, cancellationToken);
                command.Tag = tag;
                
                return tag is not null;
            })
            .WithMessage("Tag does not exist.");
        
        RuleFor(utc => utc.Name)
            .MaximumLength(255)
            .WithMessage("Tag name must not exceed 255 characters.")
            .MustAsync(async (command, name, cancellationToken) =>
            {
                if (name == command.Tag?.Name)
                    return true;

                var existingTag = await tagRepository.GetByNameAsync(name, cancellationToken);
                return existingTag is null;
            })
            .WithMessage("A tag with the same name already exists.");
    }
}