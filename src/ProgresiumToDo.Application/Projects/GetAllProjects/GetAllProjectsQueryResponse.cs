namespace ProgresiumToDo.Application.Projects.GetAllProjects;

public sealed record GetAllProjectsQueryResponse(
    string Message,
    IEnumerable<ProjectDto> Projects);

public sealed record ProjectDto(
    Guid Id,
    string Name,
    string? Description);
