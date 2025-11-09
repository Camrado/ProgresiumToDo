using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Domain.Auth;

public static class RefreshTokenErrors
{
    public static Error InvalidToken => new(
        "RefreshToken.Invalid",
        "Invalid or expired refresh token.");
}