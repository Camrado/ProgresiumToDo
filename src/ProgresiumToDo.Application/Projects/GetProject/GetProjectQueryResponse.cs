using ProgresiumToDo.Application.Projects.CreateProject;

namespace ProgresiumToDo.Application.Projects.GetProject;

public sealed record GetProjectQueryResponse(
    string Message,
    ProjectResponse Project);