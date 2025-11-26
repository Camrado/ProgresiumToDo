namespace ProgresiumToDo.Application.Abstractions.OAuth;

public record GoogleIdentityResult(string Sub, string Email, string FirstName, string LastName);