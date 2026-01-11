using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Projects.Repositories;

namespace ProgresiumToDo.Application.Projects.Commands.UpdateProject;

internal sealed class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserContext _userContext;
    
    public UpdateProjectCommandValidator(IProjectRepository projectRepository, IUserContext userContext)
    {
        _projectRepository = projectRepository;
        _userContext = userContext;
        
        RuleFor(upc => upc.ProjectId)
            .NotEmpty()
            .MustAsync(async (command, projectId, cancellationToken) =>
            {
                var project = await _projectRepository.GetByIdAndUserIdAsync(projectId, _userContext.UserId, cancellationToken);
                command.Project = project;
                
                return project != null;
            }).WithMessage("ProjectDetails not found.");
        
        RuleFor(upc => upc.Name)
            .MustAsync(async (command, name, cancellationToken) =>
            {
                if (name is null)
                    return true;
                
                if (command.Project?.Name == name)
                    return true;
                
                var existingProject = await _projectRepository.GetByNameAndUserIdAsync(name, _userContext.UserId, cancellationToken);
                return existingProject == null;
            }).WithMessage("A project with this name already exists.");
    }
}
