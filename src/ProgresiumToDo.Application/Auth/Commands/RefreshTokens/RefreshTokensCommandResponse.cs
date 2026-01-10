namespace ProgresiumToDo.Application.Auth.Commands.RefreshTokens;

public record RefreshTokensCommandResponse(
    string Message,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);