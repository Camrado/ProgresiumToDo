using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Projects.Repositories;

namespace ProgresiumToDo.Application.Projects.Queries.GetProject;

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
            }).WithMessage("ProjectDetails not found.");
    }
}