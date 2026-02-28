using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Auth.Commands.SendForgotPasswordEmail;

public sealed record SendForgotPasswordEmailCommand(string Email) : ICommand<SendForgotPasswordEmailCommandResponse>, INonTransactionalCommand;
