using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Domain.Auth;

public static class UserErrors
{
    public static Error UserNotFound => new(
        "User.NotFound",
        "The specified user was not found.");
    
    public static Error InvalidCredentials => new(
        "User.InvalidCredentials",
        "The provided credentials are invalid.");
    
    public static Error EmailVerificationFailed => new(
        "User.EmailVerificationFailed",
        "Email verification failed.");

    public static Error EmailAlreadyVerified => new(
        "User.EmailAlreadyVerified",
        "The email address has already been verified.");
}