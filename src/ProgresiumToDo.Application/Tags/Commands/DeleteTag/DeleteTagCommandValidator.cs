using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Projects.Repositories;
using ProgresiumToDo.Application.Tags.Repositories;

namespace ProgresiumToDo.Application.Tags.Commands.DeleteTag;

internal sealed class DeleteTagCommandValidator : AbstractValidator<DeleteTagCommand>
{
    public DeleteTagCommandValidator(IProjectRepository projectRepository, IUserContext userContext, ITagRepository tagRepository)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        
        RuleFor(dtc => dtc.ProjectId)
            .NotEmpty()
            .WithMessage("ProjectId must not be empty.")
            .MustAsync(async (command, projectId, cancellationToken) =>
            {
                var project = await projectRepository.GetByIdAndUserIdAsync(projectId, userContext.UserId, cancellationToken);
                
                return project is not null;
            })
            .WithMessage("Project does not exist.");
        
        RuleFor(dtc => dtc.TagId)
            .NotEmpty()
            .WithMessage("TagId must not be empty.")
            .MustAsync(async (command, tagId, cancellationToken) =>
            {
                var tag = await tagRepository.GetByIdAndProjectIdAsync(tagId, command.ProjectId, cancellationToken);
                command.Tag = tag;
                
                return tag is not null;
            })
            .WithMessage("Tag does not exist.");
    }
}