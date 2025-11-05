using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Auth.RegisterUser;

public sealed record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password) : ICommand<RegisterUserCommandResponse>;