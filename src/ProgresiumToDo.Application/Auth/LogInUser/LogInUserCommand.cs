using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Auth.LogInUser;

public sealed record LogInUserCommand(string Email, string Password) : ICommand<LogInUserCommandResponse>;