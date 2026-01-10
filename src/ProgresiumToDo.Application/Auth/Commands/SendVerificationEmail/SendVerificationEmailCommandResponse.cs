namespace ProgresiumToDo.Application.Auth.Commands.SendVerificationEmail;

public sealed record SendVerificationEmailCommandResponse(string Message, string VerificationUrl); // TODO: Remove VerificationUrl from response after implementing email service