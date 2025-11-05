namespace ProgresiumToDo.Application.Abstractions.Identity;

public sealed record AuthenticationResult(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn);