using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Domain.Tags;

public sealed class Tag : BaseEntity
{
    public string Name { get; private set; }
    
    public ICollection<TaskItem> TaskItems { get; private set; } = new List<TaskItem>();
    
    private Tag(string name)
    {
        Name = name;
    }
    
    public static Tag Create(string name)
    {
        return new Tag(name);
    }
    
    public void Update(string? name)
    {
        if (name is not null)
            Name = name;
    }
}