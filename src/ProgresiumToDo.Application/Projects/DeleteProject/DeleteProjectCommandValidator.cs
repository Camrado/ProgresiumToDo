using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Projects.DeleteProject;

internal sealed class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserContext _userContext;
    
    public DeleteProjectCommandValidator(IProjectRepository projectRepository, IUserContext userContext)
    {
        _projectRepository = projectRepository;
        _userContext = userContext;
        
        RuleFor(dpc => dpc.ProjectId)
            .NotEmpty()
            .MustAsync(async (command, projectId, cancellationToken) =>
            {
                var project = await _projectRepository.GetByIdAndUserIdAsync(projectId, _userContext.UserId, cancellationToken);
                command.Project = project;
                
                return project != null;
            }).WithMessage("Project not found.");
    }
}
