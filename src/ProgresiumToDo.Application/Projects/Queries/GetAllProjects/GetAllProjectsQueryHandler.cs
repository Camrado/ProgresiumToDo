using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Projects.Repositories;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Projects.Queries.GetAllProjects;

internal sealed class GetAllProjectsQueryHandler : IQueryHandler<GetAllProjectsQuery, GetAllProjectsQueryResponse>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserContext _userContext;

    public GetAllProjectsQueryHandler(IProjectRepository projectRepository, IUserContext userContext)
    {
        _projectRepository = projectRepository;
        _userContext = userContext;
    }

    public async Task<Result<GetAllProjectsQueryResponse>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await _projectRepository.GetAllByUserIdAsync(_userContext.UserId, cancellationToken);
        
        var projectDtos = projects
            .Select(p => new ProjectItemDto(p.Id, p.Name, p.Description))
            .ToList();
        
        return new GetAllProjectsQueryResponse("Projects retrieved successfully.", projectDtos);
    }
}
