using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tags.UpdateTag;

internal sealed class UpdateTagCommandValidator : AbstractValidator<UpdateTagCommand>
{
    public UpdateTagCommandValidator(IProjectRepository projectRepository, IUserContext userContext,
        ITagRepository tagRepository)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        
        RuleFor(utc => utc.ProjectId)
            .NotEmpty()
            .WithMessage("ProjectId must not be empty.")
            .MustAsync(async (command, projectId, cancellationToken) =>
            {
                var project = await projectRepository.GetByIdAndUserIdAsync(projectId, userContext.UserId, cancellationToken);
                
                return project is not null;
            })
            .WithMessage("Project does not exist.");
        
        RuleFor(utc => utc.TagId)
            .NotEmpty()
            .WithMessage("TagId must not be empty.")
            .MustAsync(async (command, tagId, cancellationToken) =>
            {
                var tag = await tagRepository.GetByIdAndProjectIdAsync(tagId, command.ProjectId, cancellationToken);
                command.Tag = tag;
                
                return tag is not null;
            })
            .WithMessage("Tag does not exist.");
        
        RuleFor(utc => utc.Name)
            .MaximumLength(255)
            .WithMessage("Tag name must not exceed 255 characters.");
        
        RuleFor(utc => utc.Color)
            .Matches("^#[0-9A-Fa-f]{6}$")
            .WithMessage("Color must be a valid hex value like #000000");
    }
}