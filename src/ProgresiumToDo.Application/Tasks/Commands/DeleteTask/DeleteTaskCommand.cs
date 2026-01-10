using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.Commands.DeleteTask;

public sealed record DeleteTaskCommand(Guid TaskId) : ICommand<DeleteTaskCommandResponse>
{
    internal TaskItem? TaskItem { get; set; }
}