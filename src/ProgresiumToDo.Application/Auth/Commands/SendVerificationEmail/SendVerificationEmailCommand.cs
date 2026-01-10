using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Auth.Commands.SendVerificationEmail;

public sealed record SendVerificationEmailCommand() : ICommand<SendVerificationEmailCommandResponse>;