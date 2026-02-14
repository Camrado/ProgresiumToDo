using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Domain.Projects;

public sealed class Project : BaseEntity
{
    public string Name { get; private set; }
    
    public string? Description { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public User User { get; private set; }
    
    
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

    public void Update(string? name, string? description)
    {
        if (name is not null)
        {
            Name = name;
        }

        if (description is not null)
        {
            Description = description;
        }
    }
}