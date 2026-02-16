using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.Repositories;

public interface ITaskOrderRepository
{
    void Add(TaskOrder taskOrder);

    void Delete(TaskOrder taskOrder);
    
    void DeleteRange(IEnumerable<TaskOrder> taskOrders);

    Task<TaskOrder> GetByTaskIdAndOrderTypeAsync(Guid taskId, OrderType orderType, bool trackChanges = false,
        CancellationToken cancellationToken = default);

    Task<List<TaskOrder>> GetByTaskId(Guid taskId, bool trackChanges = false, CancellationToken cancellationToken = default);
    
    Task<decimal> GetMaxOrderIndexByDueDateAsync(DateOnly dueDate, CancellationToken cancellationToken);

    Task<decimal> GetMaxOrderIndexByProjectAsync(Guid projectId, CancellationToken cancellationToken);
    
    Task<decimal> GetMaxOrderIndexByParentTaskAsync(Guid parentTaskId, CancellationToken cancellationToken);
}