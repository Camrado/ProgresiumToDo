using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProgresiumToDo.Application.Projects.Commands.CreateProject;
using ProgresiumToDo.Application.Projects.Commands.DeleteProject;
using ProgresiumToDo.Application.Projects.Commands.UpdateProject;
using ProgresiumToDo.Application.Projects.Queries.GetAllProjects;
using ProgresiumToDo.Application.Projects.Queries.GetProject;
using ProgresiumToDo.Infrastructure.Auth.Authentication;

namespace ProgresiumToDo.API.Controllers.Tasks;

[Route("api/progresium-todo/v1/projects")]
public class ProjectController : ApiControllerBase
{
    public ProjectController(IMediator mediator) : base(mediator)
    {
    }

    [AuthorizeVerified]
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectCommand createProjectCommand,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(createProjectCommand, cancellationToken);
        return FromResult(result);
    }

    [AuthorizeVerified]
    [HttpGet("{projectId}")]
    public async Task<IActionResult> GetProjectById([FromRoute] Guid projectId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetProjectQuery(projectId), cancellationToken);
        return FromResult(result);
    }
    
    [AuthorizeVerified]
    [HttpGet]
    public async Task<IActionResult> GetAllProjects(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAllProjectsQuery(), cancellationToken);
        return FromResult(result);
    }

    [AuthorizeVerified]
    [HttpPatch("{projectId}")]
    public async Task<IActionResult> UpdateProject([FromRoute] Guid projectId,
        [FromBody] UpdateProjectCommand updateProjectCommand,
        CancellationToken cancellationToken)
    {
        updateProjectCommand.ProjectId = projectId;
        var result = await Mediator.Send(updateProjectCommand, cancellationToken);
        return FromResult(result);
    }

    [AuthorizeVerified]
    [HttpDelete("{projectId}")]
    public async Task<IActionResult> DeleteProject([FromRoute] Guid projectId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeleteProjectCommand(projectId), cancellationToken);
        return FromResult(result);
    }
}