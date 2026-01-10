using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Projects;

namespace ProgresiumToDo.Application.Projects.Commands.UpdateProject;

public sealed record UpdateProjectCommand(string? Name, string? Description) : ICommand<UpdateProjectCommandResponse>
{
    public Guid ProjectId { get; set; }
    
    internal Project? Project { get; set; }
};
