using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Users.Commands.DeleteAccount;

public sealed record DeleteAccountCommand() : ICommand<DeleteAccountCommandResponse>;