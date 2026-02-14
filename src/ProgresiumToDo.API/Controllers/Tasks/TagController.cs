using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProgresiumToDo.Application.Tags.Commands.CreateTag;
using ProgresiumToDo.Application.Tags.Commands.DeleteTag;
using ProgresiumToDo.Application.Tags.Commands.UpdateTag;
using ProgresiumToDo.Application.Tags.Queries.GetAllTags;
using ProgresiumToDo.Application.Tags.Queries.GetSingleTag;
using ProgresiumToDo.Infrastructure.Services.Auth.Authentication;

namespace ProgresiumToDo.API.Controllers.Tasks;

[Route("api/progresium-todo/v1/tags")]
public class TagController : ApiControllerBase
{
    public TagController(IMediator mediator) : base(mediator)
    {
    }

    [AuthorizeVerified]
    [HttpPost]
    public async Task<IActionResult> CreateTag([FromBody] CreateTagCommand createTagCommand,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(createTagCommand, cancellationToken);
        return FromResult(result);
    }
    
    [AuthorizeVerified]
    [HttpGet]
    public async Task<IActionResult> GetAllTagsForProject(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAllTagsQuery(), cancellationToken);
        return FromResult(result);
    }

    [AuthorizeVerified]
    [HttpGet("{tagId:guid}")]
    public async Task<IActionResult> GetTagById([FromRoute] GetSingleTagQuery getSingleTagQuery, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(getSingleTagQuery, cancellationToken);
        return FromResult(result);
    }
    
    [AuthorizeVerified]
    [HttpPatch("{tagId:guid}")]
    public async Task<IActionResult> UpdateTag([FromRoute] Guid tagId,
        [FromBody] UpdateTagCommand updateTagCommand, CancellationToken cancellationToken)
    {
        updateTagCommand.TagId = tagId;
        var result = await Mediator.Send(updateTagCommand, cancellationToken);
        return FromResult(result);
    }

    [AuthorizeVerified]
    [HttpDelete("{tagId:guid}")]
    public async Task<IActionResult> DeleteTag([FromRoute] DeleteTagCommand deleteTagCommand, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(deleteTagCommand, cancellationToken);
        return FromResult(result);
    }
}