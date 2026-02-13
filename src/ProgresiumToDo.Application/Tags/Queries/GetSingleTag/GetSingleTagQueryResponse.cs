namespace ProgresiumToDo.Application.Tags.Queries.GetSingleTag;

public sealed record GetSingleTagQueryResponse(
    string Message,
    TagDto Tag);
    
public sealed record TagDto(
    Guid Id,
    string Name,
    string Color,
    DateTime CreatedAt,
    DateTime UpdatedAt);