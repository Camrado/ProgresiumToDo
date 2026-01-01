using ProgresiumToDo.Application.Abstractions.Tasks;
using TaskStatus = ProgresiumToDo.Domain.Tasks.TaskStatus;

namespace ProgresiumToDo.Infrastructure.Tasks;

internal sealed class TaskStatusPolicy : ITaskStatusPolicy
{
    public bool HasLogicalGroupChanged(TaskStatus oldStatus, TaskStatus newStatus)
    {
        return GetGroup(oldStatus) != GetGroup(newStatus);
    }
    
    public bool HasLogicalGroupStayedAsInProgress(TaskStatus oldStatus, TaskStatus newStatus)
    {
        return GetGroup(oldStatus) == TaskStatusGroup.InProgress && GetGroup(newStatus) == TaskStatusGroup.InProgress;
    }
    
    private TaskStatusGroup GetGroup(TaskStatus status) => status switch
    {
        TaskStatus.Pending => TaskStatusGroup.InProgress,
        TaskStatus.InProgress => TaskStatusGroup.InProgress,
        TaskStatus.Completed => TaskStatusGroup.Finished,
        TaskStatus.Cancelled => TaskStatusGroup.Finished,
        _ => throw new ArgumentOutOfRangeException(nameof(status))
    };

    private enum TaskStatusGroup { InProgress, Finished }
}