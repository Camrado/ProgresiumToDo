using ProgresiumToDo.Application.Projects.GetProject;

namespace ProgresiumToDo.Application.Projects.CreateProject;

public sealed record CreateProjectCommandResponse(
    string Message,
    ProjectDetailsDto ProjectDetails);