using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.UpdateSubtask;

public sealed record UpdateSubtaskCommand(
    string? Title,
    string? Description,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    string? Priority,
    string? Status,
    decimal? OrderIndex
) : ICommand<UpdateSubtaskCommandResponse>
{
    public Guid ParentTaskId { get; set; }
    
    public Guid SubtaskId { get; set; }
    
    internal TaskItem? SubtaskItem { get; set; }
}