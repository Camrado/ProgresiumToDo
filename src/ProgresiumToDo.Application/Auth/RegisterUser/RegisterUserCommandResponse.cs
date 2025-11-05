namespace ProgresiumToDo.Application.Auth.RegisterUser;

public sealed record RegisterUserCommandResponse(
    string Message,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);