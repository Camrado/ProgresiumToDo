using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgresiumToDo.Application.Tasks.CreateTask;
using ProgresiumToDo.Application.Tasks.DeleteTask;
using ProgresiumToDo.Application.Tasks.GetSingleTask;
using ProgresiumToDo.Application.Tasks.GetTasks;
using ProgresiumToDo.Application.Tasks.UpdateTask;

namespace ProgresiumToDo.API.Controllers.Tasks;

[Route("api/progresium-todo/v1/tasks")]
public class TaskController : ApiControllerBase
{
    private readonly IMediator _mediator;
    
    public TaskController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskCommand createTaskCommand,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(createTaskCommand, cancellationToken);
        return FromResult(result);
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetTasks([FromQuery] GetTasksQuery getTasksQuery, 
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(getTasksQuery, cancellationToken);
        return FromResult(result);
    }

    [Authorize]
    [HttpGet("{taskId:guid}")]
    public async Task<IActionResult> GetSingleTask([FromRoute] Guid taskId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSingleTaskQuery(taskId), cancellationToken);
        return FromResult(result);
    }

    [Authorize]
    [HttpDelete("{taskId:guid}")]
    public async Task<IActionResult> DeleteTask([FromRoute] Guid taskId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteTaskCommand(taskId), cancellationToken);
        return FromResult(result);
    }

    [Authorize]
    [HttpPatch("{taskId:guid}")]
    public async Task<IActionResult> UpdateTask([FromRoute] Guid taskId,
        [FromBody] UpdateTaskCommand updateTaskCommand, CancellationToken cancellationToken)
    {
        updateTaskCommand.TaskId = taskId;
        var result = await _mediator.Send(updateTaskCommand, cancellationToken);
        return FromResult(result);
    }
}