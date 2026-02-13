using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Domain.Tags;

public sealed class Tag : BaseEntity
{
    public string Name { get; private set; }
    
    public string Color { get; private set; }
    
    public ICollection<TaskItem> TaskItems { get; private set; } = new List<TaskItem>();
    
    private Tag(string name, string color)
    {
        Name = name;
        Color = color;
    }
    
    public static Tag Create(string name, string color)
    {
        return new Tag(name, color);
    }
    
    public void Update(string? name, string? color)
    {
        if (name is not null)
            Name = name;

        if (color is not null)
            Color = color;
    }
}