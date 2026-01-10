namespace ProgresiumToDo.Application.Auth.Commands.RegisterUser;

public sealed record RegisterUserCommandResponse(
    string Message,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);