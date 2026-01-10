using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Projects.Commands.CreateProject;

public sealed record CreateProjectCommand(string Name, string? Description) : ICommand<CreateProjectCommandResponse>;