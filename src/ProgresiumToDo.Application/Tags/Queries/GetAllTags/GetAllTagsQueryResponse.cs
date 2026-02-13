namespace ProgresiumToDo.Application.Tags.Queries.GetAllTags;

public sealed record GetAllTagsQueryResponse(
    string Message,
    IEnumerable<TagListItemDto> Tags);
    
public sealed record TagListItemDto(
    Guid Id,
    string Name,
    string Color,
    DateTime CreatedAt,
    DateTime UpdatedAt);