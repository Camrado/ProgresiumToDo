namespace ProgresiumToDo.Domain.Tasks;

public interface ITaskItemRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    void Add(TaskItem taskItem);
}
