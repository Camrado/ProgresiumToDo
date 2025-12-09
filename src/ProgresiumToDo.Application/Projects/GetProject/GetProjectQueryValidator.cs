using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Projects.GetProject;

internal sealed class GetProjectQueryValidator : AbstractValidator<GetProjectQuery>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserContext _userContext;
    
    public GetProjectQueryValidator(IProjectRepository projectRepository, IUserContext userContext)
    {
        _projectRepository = projectRepository;
        _userContext = userContext;
        
        RuleFor(gpq => gpq.ProjectId)
            .NotEmpty()
            .MustAsync(async (query, projectId, cancellationToken) =>
            {
                var project = await _projectRepository.GetByIdAndUserIdAsync(projectId, _userContext.UserId, cancellationToken);
                query.Project = project;
                
                return project != null;
            }).WithMessage("Project not found.");
    }
}