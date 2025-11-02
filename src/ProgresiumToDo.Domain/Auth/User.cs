using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Billing;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Domain.Auth;

public sealed class User : BaseEntity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();
    
    public ICollection<Subscription> Subscriptions { get; private set; } = new List<Subscription>();
    
    public ICollection<FeatureUsage.FeatureUsage> FeatureUsages { get; private set; } = new List<FeatureUsage.FeatureUsage>();
    
    public ICollection<Project> Projects { get; private set; } = new List<Project>();
    
    public ICollection<TaskItem> TaskItems { get; private set; } = new List<TaskItem>();
    
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