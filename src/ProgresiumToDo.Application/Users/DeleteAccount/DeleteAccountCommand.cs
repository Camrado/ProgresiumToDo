using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Users.DeleteAccount;

public sealed record DeleteAccountCommand() : ICommand<DeleteAccountCommandResponse>;