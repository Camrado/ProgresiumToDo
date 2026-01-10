namespace ProgresiumToDo.Application.Tags.Commands.CreateTag;

public sealed record CreateTagCommandResponse(
    string Message,
    CreatedTagDto Tag);
    
public sealed record CreatedTagDto(
    Guid Id,
    string Name,
    string Color,
    Guid ProjectId,
    DateTime CreatedAt);