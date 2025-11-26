using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.OAuth.GoogleCallbackOAuth;

public sealed record GoogleCallbackOAuthCommand(string Code, string State) : ICommand<GoogleCallbackOAuthCommandResponse>;