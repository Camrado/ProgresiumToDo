using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Projects.DeleteProject;

internal sealed class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    public DeleteProjectCommandValidator(IProjectRepository projectRepository, IUserContext userContext)
    {
        RuleFor(dpc => dpc.ProjectId)
            .NotEmpty()
            .MustAsync(async (command, projectId, cancellationToken) =>
            {
                var project = await projectRepository.GetByIdAndUserIdAsync(projectId, userContext.UserId, cancellationToken);
                command.Project = project;
                
                return project != null;
            }).WithMessage("ProjectDetails not found.");
    }
}
