namespace ProgresiumToDo.Application.Auth.SendVerificationEmail;

public sealed record SendVerificationEmailCommandResponse(string Message, string VerificationUrl); // TODO: Remove VerificationUrl from response after implementing email service