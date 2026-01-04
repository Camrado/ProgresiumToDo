using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.RemoveTagFromTask;

public sealed record RemoveTagFromTaskCommand(Guid TagId) : ICommand<RemoveTagFromTaskCommandResponse>
{
    public Guid TaskId { get; set; }
    
    internal Tag? Tag { get; set; }
    internal TaskItem? TaskItem { get; set; }
}