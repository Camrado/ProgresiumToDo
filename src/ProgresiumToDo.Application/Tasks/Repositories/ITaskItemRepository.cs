using ProgresiumToDo.Application.Tasks.Repositories.DTOs;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.Repositories;

public interface ITaskItemRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<TaskItem?> GetByIdWithTagsIncludedAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    
    Task<List<TaskItemWithOrder>> GetAllByUserIdIncludingProjectSubtasksTagsAsync(TaskQueryFilter filter, CancellationToken cancellationToken = default);
    
    Task<TaskItemWithOrder?> GetByIdIncludingProjectSubtasksTagsAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    
    void Add(TaskItem taskItem);

    void Delete(TaskItem taskItem);
}
