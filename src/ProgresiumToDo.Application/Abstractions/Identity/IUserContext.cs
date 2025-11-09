namespace ProgresiumToDo.Application.Abstractions.Identity;

public interface IUserContext
{
    Guid UserId { get; }
    
    string Email { get; }
}