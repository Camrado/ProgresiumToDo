using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Domain.Auth;

public static class OAuthErrors
{
    public static Error InvalidOrExpiredState = new(
        "OAuth.InvalidOrExpiredState",
        "The provided state is invalid or has expired.");
    
    public static Error CannotLinkGoogleAccount = new(
        "OAuth.CannotLinkGoogleAccount",
        "Cannot link Google account.");
}