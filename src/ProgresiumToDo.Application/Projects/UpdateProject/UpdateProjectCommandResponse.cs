using ProgresiumToDo.Application.Projects.GetProject;

namespace ProgresiumToDo.Application.Projects.UpdateProject;

public sealed record UpdateProjectCommandResponse(
    string Message,
    ProjectDetailsDto ProjectDetails);
