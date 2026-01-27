using ProgresiumToDo.Application.Abstractions.Auth.Entitlement;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.FeatureUsage;
using ProgresiumToDo.Domain.Tags;

namespace ProgresiumToDo.Application.Tasks.Commands.CreateTask;

public sealed record CreateTaskCommand(
    Guid? ProjectId,
    string Title,
    string? Description,
    string? Priority,
    DateOnly? DueDate,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    string? Status,
    List<Guid> TagIds) : ICommand<CreateTaskCommandResponse>, IEntitledRequest
{
    internal List<Tag> Tags { get; init; } = [];
    
    public IEnumerable<FeatureName> GetRequiredEntitlements()
    {
        if (StartTime.HasValue || EndTime.HasValue)
        {
            yield return FeatureName.TaskDuration;
        }
    }
}
