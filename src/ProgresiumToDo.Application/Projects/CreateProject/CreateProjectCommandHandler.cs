using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Projects.GetProject;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Projects.CreateProject;

internal sealed class CreateProjectCommandHandler : ICommandHandler<CreateProjectCommand, CreateProjectCommandResponse>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserContext _userContext;
    
    public CreateProjectCommandHandler(IProjectRepository projectRepository, IUserContext userContext)
    {
        _projectRepository = projectRepository;
        _userContext = userContext;
    }
    
    public Task<Result<CreateProjectCommandResponse>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = Project.Create(request.Name, request.Description, _userContext.UserId);
        
        _projectRepository.Add(project);

        return Task.FromResult<Result<CreateProjectCommandResponse>>(new CreateProjectCommandResponse("ProjectDetails created successfully.",
            new ProjectDetailsDto(project.Id, project.Name, project.Description, project.CreatedAt, project.UpdatedAt)));
    }
}