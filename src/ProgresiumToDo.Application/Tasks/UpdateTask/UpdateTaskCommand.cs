using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.UpdateTask;

public sealed record UpdateTaskCommand(
    string? Title,
    string? Description,
    string? Priority,
    string? Status,
    DateOnly? DueDate,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    decimal? OrderIndex,
    Guid? ProjectId) : ICommand<UpdateTaskCommandResponse>
{
    public Guid TaskId { get; set; }
    
    internal TaskItem? TaskItem { get; set; }
}