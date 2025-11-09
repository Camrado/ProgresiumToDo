using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Auth.LogOutUser;

public sealed record LogOutUserCommand(string RefreshToken) : ICommand<LogOutUserCommandResponse>;