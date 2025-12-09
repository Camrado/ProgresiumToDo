using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Projects.UpdateProject;

public sealed record UpdateProjectCommand(string? Name, string? Description) : ICommand<UpdateProjectCommandResponse>
{
    public Guid ProjectId { get; set; }
    
    internal Project? Project { get; set; }
};
