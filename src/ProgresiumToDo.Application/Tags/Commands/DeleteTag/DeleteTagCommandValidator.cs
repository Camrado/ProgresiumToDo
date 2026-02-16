using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Tags.Repositories;

namespace ProgresiumToDo.Application.Tags.Commands.DeleteTag;

internal sealed class DeleteTagCommandValidator : AbstractValidator<DeleteTagCommand>
{
    public DeleteTagCommandValidator(ITagRepository tagRepository, IUserContext userContext)
    {   
        RuleFor(dtc => dtc.TagId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("TagId must not be empty.")
            .MustAsync(async (command, tagId, cancellationToken) =>
            {
                var tag = await tagRepository.GetByIdAndUserIdAsync(tagId, userContext.UserId, trackChanges: true, cancellationToken);
                command.Tag = tag;
                
                return tag is not null;
            })
            .WithMessage("Tag does not exist.");
    }
}