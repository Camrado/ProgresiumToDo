using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.AddTagToTask;

public sealed record AddTagToTaskCommand(Guid TagId) : ICommand<AddTagToTaskCommandResponse>
{
    public Guid TaskId { get; set; }
    
    internal Tag? Tag { get; set; }
    internal TaskItem? TaskItem { get; set; }
}