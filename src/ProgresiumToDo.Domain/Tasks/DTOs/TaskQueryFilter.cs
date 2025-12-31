namespace ProgresiumToDo.Domain.Tasks.DTOs;

public sealed record TaskQueryFilter(
    Guid UserId,
    Guid? ProjectId = null,
    DateOnly? DueDateFrom = null,
    DateOnly? DueDateTo = null,
    OrderType? OrderType = null,
    int? Page = null,
    int? PageSize = null,
    string? SortBy = null,
    string? SortOrder = null
);