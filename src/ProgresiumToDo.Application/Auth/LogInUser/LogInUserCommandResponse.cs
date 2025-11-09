namespace ProgresiumToDo.Application.Auth.LogInUser;

public sealed record LogInUserCommandResponse(
    string Message,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);