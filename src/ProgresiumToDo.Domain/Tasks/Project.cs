using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Domain.Tasks;

public sealed class Project : BaseEntity
{
    public string Name { get; private set; }
    
    public string? Description { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public User User { get; private set; }
}