using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.OAuth.Commands.StartOAuth;

public sealed record StartOAuthCommand(string Provider) : ICommand<StartOAuthCommandResponse>;