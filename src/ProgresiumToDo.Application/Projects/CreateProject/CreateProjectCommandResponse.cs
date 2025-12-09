namespace ProgresiumToDo.Application.Projects.CreateProject;

public sealed record CreateProjectCommandResponse(
    string Message,
    ProjectResponse Project);

public sealed record ProjectResponse(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdatedAt);