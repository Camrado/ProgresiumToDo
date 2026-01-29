namespace ProgresiumToDo.Application.Abstractions.Auth.Identity;

public interface IUserContext
{
    Guid UserId { get; }
    
    string Email { get; }
}