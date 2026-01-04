using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgresiumToDo.Application.Tasks.AddTagToTask;
using ProgresiumToDo.Application.Tasks.CreateSubtask;
using ProgresiumToDo.Application.Tasks.CreateTask;
using ProgresiumToDo.Application.Tasks.DeleteTask;
using ProgresiumToDo.Application.Tasks.GetSingleTask;
using ProgresiumToDo.Application.Tasks.GetTasks;
using ProgresiumToDo.Application.Tasks.RemoveTagFromTask;
using ProgresiumToDo.Application.Tasks.UpdateSubtask;
using ProgresiumToDo.Application.Tasks.UpdateTask;

namespace ProgresiumToDo.API.Controllers.Tasks;

[Route("api/progresium-todo/v1/tasks")]
public class TaskController : ApiControllerBase
{
    public TaskController(IMediator mediator) : base(mediator)
    {
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskCommand createTaskCommand,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(createTaskCommand, cancellationToken);
        return FromResult(result);
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetTasks([FromQuery] GetTasksQuery getTasksQuery, 
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(getTasksQuery, cancellationToken);
        return FromResult(result);
    }

    [Authorize]
    [HttpGet("{taskId:guid}")]
    public async Task<IActionResult> GetSingleTask([FromRoute] Guid taskId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetSingleTaskQuery(taskId), cancellationToken);
        return FromResult(result);
    }

    [Authorize]
    [HttpDelete("{taskId:guid}")]
    public async Task<IActionResult> DeleteTask([FromRoute] Guid taskId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeleteTaskCommand(taskId), cancellationToken);
        return FromResult(result);
    }

    [Authorize]
    [HttpPatch("{taskId:guid}")]
    public async Task<IActionResult> UpdateTask([FromRoute] Guid taskId,
        [FromBody] UpdateTaskCommand updateTaskCommand, CancellationToken cancellationToken)
    {
        updateTaskCommand.TaskId = taskId;
        var result = await Mediator.Send(updateTaskCommand, cancellationToken);
        return FromResult(result);
    }
    
    [Authorize]
    [HttpPost("{taskId:guid}/subtasks")]
    public async Task<IActionResult> CreateSubTask([FromRoute] Guid taskId,
        [FromBody] CreateSubtaskCommand createTaskCommand, CancellationToken cancellationToken)
    {
        createTaskCommand.ParentTaskId = taskId;
        var result = await Mediator.Send(createTaskCommand, cancellationToken);
        return FromResult(result);
    }
    
    [Authorize]
    [HttpPatch("{parentTaskId:guid}/subtasks/{subtaskId:guid}")]
    public async Task<IActionResult> UpdateSubTask([FromRoute] Guid parentTaskId, [FromRoute] Guid subtaskId,
        [FromBody] UpdateSubtaskCommand updateSubtaskCommand, CancellationToken cancellationToken)
    {
        updateSubtaskCommand.ParentTaskId = parentTaskId;
        updateSubtaskCommand.SubtaskId = subtaskId;
        var result = await Mediator.Send(updateSubtaskCommand, cancellationToken);
        return FromResult(result);
    }
    
    [Authorize]
    [HttpPost("{taskId:guid}/tags/{tagId:guid}")]
    public async Task<IActionResult> AddTagToTask([FromRoute] Guid taskId,
        [FromRoute] AddTagToTaskCommand addTagToTaskCommand, CancellationToken cancellationToken)
    {
        addTagToTaskCommand.TaskId = taskId;
        var result = await Mediator.Send(addTagToTaskCommand, cancellationToken);
        return FromResult(result);
    }
    
    [Authorize]
    [HttpDelete("{taskId:guid}/tags/{tagId:guid}")]
    public async Task<IActionResult> RemoveTagFromTask([FromRoute] Guid taskId,
        [FromRoute] RemoveTagFromTaskCommand removeTagFromTaskCommand, CancellationToken cancellationToken)
    {
        removeTagFromTaskCommand.TaskId = taskId;
        var result = await Mediator.Send(removeTagFromTaskCommand, cancellationToken);
        return FromResult(result);
    }
}