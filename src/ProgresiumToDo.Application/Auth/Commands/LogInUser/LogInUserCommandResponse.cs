namespace ProgresiumToDo.Application.Auth.Commands.LogInUser;

public sealed record LogInUserCommandResponse(
    string Message,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);