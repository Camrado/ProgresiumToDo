using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Domain.Auth.Errors;

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
    
    public static Error InvalidEmailVerificationCode => new(
        "User.InvalidEmailVerificationCode",
        "The provided email verification code is invalid.");
    
    public static Error EmailVerificationCodeExpired => new(
        "User.EmailVerificationCodeExpired",
        "The email verification code has expired.");

    public static Error EmailCooldown(int remainingSeconds) => new(
        "User.EmailCooldown",
        $"Please wait {remainingSeconds} seconds before requesting a new code.");
    
    public static Error InvalidPasswordResetCode => new(
        "User.InvalidPasswordResetCode",
        "The provided password reset code is invalid.");
    
    public static Error PasswordResetCodeExpired => new(
        "User.PasswordResetCodeExpired",
        "The password reset code has expired.");
    
    public static Error PasswordResetFailed => new(
        "User.PasswordResetFailed",
        "Password reset failed.");
}