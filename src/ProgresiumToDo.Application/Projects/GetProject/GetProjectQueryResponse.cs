namespace ProgresiumToDo.Application.Projects.GetProject;

public sealed record GetProjectQueryResponse(
    string Message,
    ProjectDetailsDto ProjectDetails);
    
public sealed record ProjectDetailsDto(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdatedAt);