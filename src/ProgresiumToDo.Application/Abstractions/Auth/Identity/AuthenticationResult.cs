using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Application.Abstractions.Auth.Identity;

public sealed record AuthenticationResult(
    string AccessToken,
    RefreshToken RefreshToken,
    int ExpiresIn);