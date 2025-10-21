using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Domain.FeatureUsage;

public sealed class Feature
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    
    public string Name { get; private set; }
    
    public string? Description { get; private set; }
    
    private Feature(string name, string? description)
    {
        Name = name;
        Description = description;
    }
    
    public static Feature Create(string name, string? description)
    {
        return new Feature(name, description);
    }
}