using ProgresiumToDo.Application.Abstractions.Auth.Entitlement;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.FeatureUsage;

namespace ProgresiumToDo.Application.Tasks.Commands.CreateTask;

public sealed record CreateTaskCommand(
    Guid? ProjectId,
    string Title,
    string? Description,
    string? Priority,
    DateOnly? DueDate,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    string? Status) : ICommand<CreateTaskCommandResponse>, IEntitledRequest
{
    public IEnumerable<FeatureName> GetRequiredEntitlements()
    {
        if (StartTime.HasValue || EndTime.HasValue)
        {
            yield return FeatureName.TaskDuration;
        }
    }
}
