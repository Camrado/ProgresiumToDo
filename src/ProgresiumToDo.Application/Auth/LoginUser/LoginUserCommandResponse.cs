namespace ProgresiumToDo.Application.Auth.LoginUser;

public sealed record LoginUserCommandResponse(
    string Message,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);