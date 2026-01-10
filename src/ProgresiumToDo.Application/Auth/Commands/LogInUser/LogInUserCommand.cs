using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Auth.Commands.LogInUser;

public sealed record LogInUserCommand(string Email, string Password) : ICommand<LogInUserCommandResponse>;