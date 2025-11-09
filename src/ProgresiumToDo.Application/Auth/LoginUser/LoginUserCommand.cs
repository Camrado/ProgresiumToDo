using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Auth.LoginUser;

public sealed record LoginUserCommand(string Email, string Password) : ICommand<LoginUserCommandResponse>;