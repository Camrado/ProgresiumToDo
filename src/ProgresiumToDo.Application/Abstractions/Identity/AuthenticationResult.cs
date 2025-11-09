using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Application.Abstractions.Identity;

public sealed record AuthenticationResult(
    string AccessToken,
    RefreshToken RefreshToken,
    int ExpiresIn);