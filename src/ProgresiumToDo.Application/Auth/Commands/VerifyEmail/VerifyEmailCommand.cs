using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Auth.Commands.VerifyEmail;

public sealed record VerifyEmailCommand(string UserId, string VerificationToken) : ICommand<VerifyEmailCommandResponse>;