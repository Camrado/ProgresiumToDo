using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgresiumToDo.Application.Tags.Commands.CreateTag;
using ProgresiumToDo.Application.Tags.Commands.DeleteTag;
using ProgresiumToDo.Application.Tags.Commands.UpdateTag;
using ProgresiumToDo.Application.Tags.Queries.GetAllTagsForProject;
using ProgresiumToDo.Application.Tags.Queries.GetSingleTag;

namespace ProgresiumToDo.API.Controllers.Tasks;

[Route("api/progresium-todo/v1/projects/{projectId:guid}/tags")]
public class TagController : ApiControllerBase
{
    public TagController(IMediator mediator) : base(mediator)
    {
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateTag([FromRoute] Guid projectId, [FromBody] CreateTagCommand createTagCommand,
        CancellationToken cancellationToken)
    {
        createTagCommand.ProjectId = projectId;
        var result = await Mediator.Send(createTagCommand, cancellationToken);
        return FromResult(result);
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllTagsForProject([FromRoute] Guid projectId, CancellationToken cancellationToken)
    {
        var getAllTagsForProjectQuery = new GetAllTagsForProjectQuery { ProjectId = projectId };
        var result = await Mediator.Send(getAllTagsForProjectQuery, cancellationToken);
        return FromResult(result);
    }

    [Authorize]
    [HttpGet("{tagId:guid}")]
    public async Task<IActionResult> GetTagById([FromRoute] Guid projectId,
        [FromRoute] GetSingleTagQuery getSingleTagQuery, CancellationToken cancellationToken)
    {
        getSingleTagQuery.ProjectId = projectId;
        var result = await Mediator.Send(getSingleTagQuery, cancellationToken);
        return FromResult(result);
    }
    
    [Authorize]
    [HttpPatch("{tagId:guid}")]
    public async Task<IActionResult> UpdateTag([FromRoute] Guid projectId, [FromRoute] Guid tagId,
        [FromBody] UpdateTagCommand updateTagCommand, CancellationToken cancellationToken)
    {
        updateTagCommand.ProjectId = projectId;
        updateTagCommand.TagId = tagId;
        var result = await Mediator.Send(updateTagCommand, cancellationToken);
        return FromResult(result);
    }

    [Authorize]
    [HttpDelete("{tagId:guid}")]
    public async Task<IActionResult> DeleteTag([FromRoute] Guid projectId,
        [FromRoute] DeleteTagCommand deleteTagCommand, CancellationToken cancellationToken)
    {
        deleteTagCommand.ProjectId = projectId;
        var result = await Mediator.Send(deleteTagCommand, cancellationToken);
        return FromResult(result);
    }
}