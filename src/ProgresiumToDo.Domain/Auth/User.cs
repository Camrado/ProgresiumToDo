using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Domain.Auth;

public sealed class User : BaseEntity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    
    private User(string  firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public static User Create(string firstName, string lastName)
    {
        return new User(firstName, lastName);
    }
}