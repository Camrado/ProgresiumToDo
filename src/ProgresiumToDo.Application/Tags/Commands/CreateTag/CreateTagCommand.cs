using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Tags.Commands.CreateTag;

public sealed record CreateTagCommand(
    string Name,
    string Color) : ICommand<CreateTagCommandResponse>;