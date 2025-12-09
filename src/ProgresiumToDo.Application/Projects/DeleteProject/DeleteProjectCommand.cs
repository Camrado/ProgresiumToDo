using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Projects.DeleteProject;

public sealed record DeleteProjectCommand(Guid ProjectId) : ICommand<DeleteProjectCommandResponse>
{   
    internal Project? Project { get; set; }
};
