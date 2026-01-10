namespace ProgresiumToDo.Application.Tags.Queries.GetAllTagsForProject;

public sealed record GetAllTagsForProjectQueryResponse(
    string Message,
    IEnumerable<TagListItemDto> Tags);
    
public sealed record TagListItemDto(
    Guid Id,
    string Name,
    string Color,
    DateTime CreatedAt,
    DateTime UpdatedAt);