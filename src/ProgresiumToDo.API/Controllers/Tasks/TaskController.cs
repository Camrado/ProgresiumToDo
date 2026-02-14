using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProgresiumToDo.Application.Tasks.Commands.CreateSubtask;
using ProgresiumToDo.Application.Tasks.Commands.CreateTask;
using ProgresiumToDo.Application.Tasks.Commands.DeleteTask;
using ProgresiumToDo.Application.Tasks.Commands.UpdateSubtask;
using ProgresiumToDo.Application.Tasks.Commands.UpdateTask;
using ProgresiumToDo.Application.Tasks.Queries.GetSingleTask;
using ProgresiumToDo.Application.Tasks.Queries.GetTasks;
using ProgresiumToDo.Infrastructure.Services.Auth.Authentication;

namespace ProgresiumToDo.API.Controllers.Tasks;

[Route("api/progresium-todo/v1/tasks")]
public class TaskController : ApiControllerBase
{
    public TaskController(IMediator mediator) : base(mediator)
    {
    }

    [AuthorizeVerified]
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskCommand createTaskCommand,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(createTaskCommand, cancellationToken);
        return FromResult(result);
    }
    
    [AuthorizeVerified]
    [HttpGet]
    public async Task<IActionResult> GetTasks([FromQuery] GetTasksQuery getTasksQuery, 
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(getTasksQuery, cancellationToken);
        return FromResult(result);
    }

    [AuthorizeVerified]
    [HttpGet("{taskId:guid}")]
    public async Task<IActionResult> GetSingleTask([FromRoute] Guid taskId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetSingleTaskQuery(taskId), cancellationToken);
        return FromResult(result);
    }

    [AuthorizeVerified]
    [HttpDelete("{taskId:guid}")]
    public async Task<IActionResult> DeleteTask([FromRoute] Guid taskId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeleteTaskCommand(taskId), cancellationToken);
        return FromResult(result);
    }

    [AuthorizeVerified]
    [HttpPatch("{taskId:guid}")]
    public async Task<IActionResult> UpdateTask([FromRoute] Guid taskId,
        [FromBody] UpdateTaskCommand updateTaskCommand, CancellationToken cancellationToken)
    {
        updateTaskCommand.TaskId = taskId;
        var result = await Mediator.Send(updateTaskCommand, cancellationToken);
        return FromResult(result);
    }
    
    [AuthorizeVerified]
    [HttpPost("{taskId:guid}/subtasks")]
    public async Task<IActionResult> CreateSubTask([FromRoute] Guid taskId,
        [FromBody] CreateSubtaskCommand createTaskCommand, CancellationToken cancellationToken)
    {
        createTaskCommand.ParentTaskId = taskId;
        var result = await Mediator.Send(createTaskCommand, cancellationToken);
        return FromResult(result);
    }
    
    [AuthorizeVerified]
    [HttpPatch("{parentTaskId:guid}/subtasks/{subtaskId:guid}")]
    public async Task<IActionResult> UpdateSubTask([FromRoute] Guid parentTaskId, [FromRoute] Guid subtaskId,
        [FromBody] UpdateSubtaskCommand updateSubtaskCommand, CancellationToken cancellationToken)
    {
        updateSubtaskCommand.ParentTaskId = parentTaskId;
        updateSubtaskCommand.SubtaskId = subtaskId;
        var result = await Mediator.Send(updateSubtaskCommand, cancellationToken);
        return FromResult(result);
    }
}