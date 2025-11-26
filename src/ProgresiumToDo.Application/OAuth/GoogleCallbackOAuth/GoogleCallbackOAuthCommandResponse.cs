namespace ProgresiumToDo.Application.OAuth.GoogleCallbackOAuth;

public sealed record GoogleCallbackOAuthCommandResponse(
    string Message,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);