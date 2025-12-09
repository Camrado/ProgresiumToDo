using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Projects.CreateProject;

public sealed record CreateProjectCommand(string Name, string? Description) : ICommand<CreateProjectCommandResponse>;