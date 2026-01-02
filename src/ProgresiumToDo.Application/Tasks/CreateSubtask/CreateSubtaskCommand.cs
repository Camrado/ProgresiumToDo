using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.CreateSubtask;

public sealed record CreateSubtaskCommand(
    string Title,
    string? Description,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    string? Priority,
    string? Status) : ICommand<CreateSubtaskCommandResponse>
{
    public Guid ParentTaskId { get; set; }
    
    internal TaskItem? ParentTaskItem { get; set; }
}