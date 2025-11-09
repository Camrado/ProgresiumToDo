namespace ProgresiumToDo.Application.Auth.RefreshTokens;

public record RefreshTokensCommandResponse(
    string Message,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);