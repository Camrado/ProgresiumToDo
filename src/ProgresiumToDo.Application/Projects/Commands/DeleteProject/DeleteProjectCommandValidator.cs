using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Projects.Repositories;

namespace ProgresiumToDo.Application.Projects.Commands.DeleteProject;

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
            }).WithMessage("Project not found.");
    }
}
