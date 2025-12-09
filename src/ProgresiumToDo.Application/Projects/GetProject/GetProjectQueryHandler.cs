using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Projects.CreateProject;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Projects.GetProject;

internal sealed class GetProjectQueryHandler : IQueryHandler<GetProjectQuery, GetProjectQueryResponse>
{
    public Task<Result<GetProjectQueryResponse>> Handle(GetProjectQuery request, CancellationToken cancellationToken)
    {
        var project = request.Project;
        
        return Task.FromResult<Result<GetProjectQueryResponse>>(new GetProjectQueryResponse("Project created successfully.",
            new ProjectResponse(project!.Id, project.Name, project.Description, project.CreatedAt, project.UpdatedAt)));
    }
}