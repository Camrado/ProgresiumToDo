using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Auth.SendVerificationEmail;

public sealed record SendVerificationEmailCommand() : ICommand<SendVerificationEmailCommandResponse>;