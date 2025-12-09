using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgresiumToDo.Application.Projects.CreateProject;
using ProgresiumToDo.Application.Projects.GetProject;

namespace ProgresiumToDo.API.Controllers.Tasks;

[Route("api/progresium-todo/v1/projects")]
public class ProjectController : ApiControllerBase
{
    private readonly IMediator _mediator;
    
    public ProjectController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectCommand createProjectCommand,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(createProjectCommand, cancellationToken);
        return FromResult(result);
    }

    [Authorize]
    [HttpGet("{projectId}")]
    public async Task<IActionResult> GetProjectById([FromRoute] Guid projectId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProjectQuery(projectId), cancellationToken);
        return FromResult(result);
    }
}