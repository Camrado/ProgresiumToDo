namespace ProgresiumToDo.Application.Projects.GetAllProjects;

public sealed record GetAllProjectsQueryResponse(
    string Message,
    IEnumerable<ProjectItemDto> Projects);

public sealed record ProjectItemDto(
    Guid Id,
    string Name,
    string? Description);
