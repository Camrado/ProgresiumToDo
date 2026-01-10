using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Projects;

namespace ProgresiumToDo.Application.Projects.Commands.DeleteProject;

public sealed record DeleteProjectCommand(Guid ProjectId) : ICommand<DeleteProjectCommandResponse>
{   
    internal Project? Project { get; set; }
};
