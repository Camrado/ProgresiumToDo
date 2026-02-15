using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Domain.Tags;

public sealed class Tag : BaseEntity
{
    public string Name { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public ICollection<TaskItem> TaskItems { get; private set; } = new List<TaskItem>();
    
    public User User { get; private set; }
    
    private Tag(string name, Guid userId)
    {
        Name = name;
        UserId = userId;
    }
    
    public static Tag Create(string name, Guid userId)
    {
        return new Tag(name, userId);
    }
    
    public void Update(string? name)
    {
        if (name is not null)
            Name = name;
    }
}