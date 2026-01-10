using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Auth.Commands.LogOutUser;

public sealed record LogOutUserCommand(string RefreshToken) : ICommand<LogOutUserCommandResponse>;