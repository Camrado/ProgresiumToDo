using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgresiumToDo.Application.Projects.CreateProject;
using ProgresiumToDo.Application.Projects.DeleteProject;
using ProgresiumToDo.Application.Projects.GetAllProjects;
using ProgresiumToDo.Application.Projects.GetProject;
using ProgresiumToDo.Application.Projects.UpdateProject;

namespace ProgresiumToDo.API.Controllers.Tasks;

[Route("api/progresium-todo/v1/projects")]
public class ProjectController : ApiControllerBase
{
    public ProjectController(IMediator mediator) : base(mediator)
    {
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectCommand createProjectCommand,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(createProjectCommand, cancellationToken);
        return FromResult(result);
    }

    [Authorize]
    [HttpGet("{projectId}")]
    public async Task<IActionResult> GetProjectById([FromRoute] Guid projectId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetProjectQuery(projectId), cancellationToken);
        return FromResult(result);
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllProjects(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAllProjectsQuery(), cancellationToken);
        return FromResult(result);
    }

    [Authorize]
    [HttpPatch("{projectId}")]
    public async Task<IActionResult> UpdateProject([FromRoute] Guid projectId,
        [FromBody] UpdateProjectCommand updateProjectCommand,
        CancellationToken cancellationToken)
    {
        updateProjectCommand.ProjectId = projectId;
        var result = await Mediator.Send(updateProjectCommand, cancellationToken);
        return FromResult(result);
    }

    [Authorize]
    [HttpDelete("{projectId}")]
    public async Task<IActionResult> DeleteProject([FromRoute] Guid projectId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeleteProjectCommand(projectId), cancellationToken);
        return FromResult(result);
    }
}