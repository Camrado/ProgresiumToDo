namespace ProgresiumToDo.Application.Abstractions.Auth.OAuth;

public record GoogleIdentityResult(string Sub, string Email, string FirstName, string LastName);