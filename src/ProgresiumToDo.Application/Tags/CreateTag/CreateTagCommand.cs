using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Tags.CreateTag;

public sealed record CreateTagCommand(
    string Name,
    string Color) : ICommand<CreateTagCommandResponse>
{
    public Guid ProjectId { get; set; }
}