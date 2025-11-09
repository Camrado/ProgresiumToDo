using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Auth.VerifyEmail;

public sealed record VerifyEmailCommand(string VerificationToken) : ICommand<VerifyEmailCommandResponse>;