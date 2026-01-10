namespace ProgresiumToDo.Application.OAuth.Commands.GoogleCallbackOAuth;

public sealed record GoogleCallbackOAuthCommandResponse(
    string Message,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);