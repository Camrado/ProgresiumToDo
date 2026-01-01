using TaskStatus = ProgresiumToDo.Domain.Tasks.TaskStatus;

namespace ProgresiumToDo.Application.Abstractions.Tasks;

public interface ITaskStatusPolicy
{
    bool HasLogicalGroupChanged(TaskStatus oldStatus, TaskStatus newStatus);

    bool HasLogicalGroupStayedAsInProgress(TaskStatus oldStatus, TaskStatus newStatus);
}