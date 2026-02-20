using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Abstractions.Tasks;

public sealed record TaskOrderContext
{
    public OrderType OrderType { get; init; }
    public Guid? ProjectId { get; init; }
    public DateOnly? DueDate { get; init; }
    public Guid? ParentTaskId { get; init; }
    public decimal? NextTaskOrderIndex { get; init; }
    public decimal? PreviousTaskOrderIndex { get; init; }
}