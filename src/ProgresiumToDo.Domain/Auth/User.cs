using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Billing;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Domain.Auth;

public sealed class User : BaseEntity
{
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    
    public Guid? ApplicationUserId { get; private set; }
    
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();
    
    public ICollection<Subscription> Subscriptions { get; private set; } = new List<Subscription>();
    
    public ICollection<FeatureUsage.FeatureUsage> FeatureUsages { get; private set; } = new List<FeatureUsage.FeatureUsage>();
    
    public ICollection<Project> Projects { get; private set; } = new List<Project>();
    
    public ICollection<TaskItem> TaskItems { get; private set; } = new List<TaskItem>();

    private User() {}
    
    private User(string email, string firstName, string lastName, Guid applicationUserId)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        ApplicationUserId = applicationUserId;
    }

    public static User Create(string email, string firstName, string lastName, Guid applicationUserId)
    {
        return new User(email, firstName, lastName, applicationUserId);
    }
    
    public void Update(string? firstName, string? lastName)
    {
        if (firstName is not null)
        {
            FirstName = firstName;
        }

        if (lastName is not null)
        {
            LastName = lastName;
        }
    }
}