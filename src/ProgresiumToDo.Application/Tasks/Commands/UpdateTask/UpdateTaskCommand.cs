using ProgresiumToDo.Application.Abstractions.Auth.Entitlement;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.FeatureUsage;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.Commands.UpdateTask;

public sealed record UpdateTaskCommand(
    string? Title,
    string? Description,
    string? Priority,
    string? Status,
    DateOnly? DueDate,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    decimal? PreviousTaskOrderIndex,
    decimal? NextTaskOrderIndex,
    string? OrderType,
    Guid? ProjectId,
    List<string>? Tags) : ICommand<UpdateTaskCommandResponse>, IEntitledRequest
{
    public Guid TaskId { get; set; }
    
    internal TaskItem? TaskItem { get; set; }
    
    public IEnumerable<FeatureName> GetRequiredEntitlements()
    {
        if (StartTime.HasValue || EndTime.HasValue)
        {
            yield return FeatureName.TaskDuration;
        }
    }
}