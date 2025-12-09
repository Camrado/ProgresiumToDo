using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Domain.Tasks;

public sealed class Project : BaseEntity
{
    public string Name { get; private set; }
    
    public string? Description { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public User User { get; private set; }
    
    public ICollection<Tag> Tags { get; private set; } = new List<Tag>();
    
    public ICollection<TaskItem> TaskItems { get; private set; } = new List<TaskItem>();
    
    private Project() { }

    private Project(string name, string? description, Guid userId)
    {
        Name = name;
        Description = description;
        UserId = userId;
    }

    public static Project Create(string name, string? description, Guid userId)
    {
        return new Project(name, description, userId);
    }
}