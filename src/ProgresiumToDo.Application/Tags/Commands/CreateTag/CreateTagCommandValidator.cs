using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Tags.Repositories;

namespace ProgresiumToDo.Application.Tags.Commands.CreateTag;

internal sealed class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator(ITagRepository tagRepository, IUserContext userContext)
    {   
        RuleFor(ctc => ctc.Name)
            .NotEmpty()
            .WithMessage("Tag name is required.")
            .MaximumLength(255)
            .WithMessage("Tag name must not exceed 255 characters.")
            .MustAsync(async (command, name, cancellationToken) =>
            {
                var existingTag = await tagRepository.GetByNameAsync(name, userContext.UserId, cancellationToken: cancellationToken);
                return existingTag is null;
            })
            .WithMessage("A tag with the same name already exists.");
    }
}