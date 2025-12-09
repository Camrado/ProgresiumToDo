using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Projects.CreateProject;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Projects.UpdateProject;

internal sealed class UpdateProjectCommandHandler : ICommandHandler<UpdateProjectCommand, UpdateProjectCommandResponse>
{
    public Task<Result<UpdateProjectCommandResponse>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = request.Project!;
        
        project.Update(request.Name, request.Description);
        
        return Task.FromResult<Result<UpdateProjectCommandResponse>>(
            new UpdateProjectCommandResponse(
                "Project updated successfully.",
                new ProjectResponse(project.Id, project.Name, project.Description, project.CreatedAt, project.UpdatedAt)));
    }
}
