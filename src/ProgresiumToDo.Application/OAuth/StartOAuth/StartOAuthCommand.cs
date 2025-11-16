using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.OAuth.StartOAuth;

public sealed record StartOAuthCommand(string Provider) : ICommand<StartOAuthCommandResponse>;