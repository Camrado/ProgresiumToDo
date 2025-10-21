using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Domain.Billing;

public sealed class Plan : BaseEntity
{
    public string Name { get; private set; }
    
    public string? Description { get; private set; }
    
    private Plan(string name, string? description)
    {
        Name = name;
        Description = description;
    }
    
    public static Plan Create(string name, string? description)
    {
        return new Plan(name, description);
    }
}