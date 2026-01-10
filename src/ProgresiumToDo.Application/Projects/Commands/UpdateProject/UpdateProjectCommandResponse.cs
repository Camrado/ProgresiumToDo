using ProgresiumToDo.Application.Projects.Queries.GetProject;

namespace ProgresiumToDo.Application.Projects.Commands.UpdateProject;

public sealed record UpdateProjectCommandResponse(
    string Message,
    ProjectDetailsDto ProjectDetails);
