namespace ProgresiumToDo.Domain.Waitlist;

public sealed class WaitlistEntry
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    
    public string Email { get; private set; }
    
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    
    private WaitlistEntry(string email)
    {
        Email = email;
    }
    
    public static WaitlistEntry Create(string email)
    {
        return new WaitlistEntry(email);
    }
}