namespace ProgresiumToDo.Application.Users.UpdateProfile;

public sealed record UpdateProfileCommandResponse(
    string Message,
    UserProfileUpdatedDto User);
    
public sealed record UserProfileUpdatedDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    DateTime UpdatedAt);