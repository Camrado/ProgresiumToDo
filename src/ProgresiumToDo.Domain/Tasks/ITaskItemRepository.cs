using ProgresiumToDo.Domain.Tasks.DTOs;

namespace ProgresiumToDo.Domain.Tasks;

public interface ITaskItemRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<decimal> GetMaxOrderIndexByProjectId(Guid projectId, DateOnly? dueDate, Guid? parentTaskId, CancellationToken cancellationToken = default);

    Task<List<TaskItem>> GetByUserIdIncludingProjectSubtasksTagsAsync(TaskQueryFilter filter, CancellationToken cancellationToken = default);
    
    void Add(TaskItem taskItem);
}
