using ProgresiumToDo.Application.Projects.CreateProject;

namespace ProgresiumToDo.Application.Projects.UpdateProject;

public sealed record UpdateProjectCommandResponse(
    string Message,
    ProjectResponse Project);
