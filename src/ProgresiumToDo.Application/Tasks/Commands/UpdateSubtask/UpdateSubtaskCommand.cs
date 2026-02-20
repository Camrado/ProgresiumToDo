using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.Commands.UpdateSubtask;

public sealed record UpdateSubtaskCommand(
    string? Title,
    string? Description,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    string? Priority,
    string? Status,
    decimal? PreviousTaskOrderIndex,
    decimal? NextTaskOrderIndex
) : ICommand<UpdateSubtaskCommandResponse>
{
    public Guid ParentTaskId { get; set; }
    
    public Guid SubtaskId { get; set; }
    
    internal TaskItem? SubtaskItem { get; set; }
}