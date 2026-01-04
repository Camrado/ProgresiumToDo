using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Domain.Tasks;

public sealed class Tag : BaseEntity
{
    public string Name { get; private set; }
    
    public string Color { get; private set; }
    
    public Guid ProjectId { get; private set; }
    
    public Project Project { get; private set; }
    
    public ICollection<TaskItem> TaskItems { get; private set; } = new List<TaskItem>();
    
    private Tag(string name, string color, Guid projectId)
    {
        Name = name;
        Color = color;
        ProjectId = projectId;
    }
    
    public static Tag Create(string name, string color, Guid projectId)
    {
        return new Tag(name, color, projectId);
    }
    
    public void Update(string? name, string? color)
    {
        if (name is not null)
            Name = name;

        if (color is not null)
            Color = color;
    }
}