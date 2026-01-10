using ProgresiumToDo.Application.Projects.Queries.GetProject;

namespace ProgresiumToDo.Application.Projects.Commands.CreateProject;

public sealed record CreateProjectCommandResponse(
    string Message,
    ProjectDetailsDto ProjectDetails);