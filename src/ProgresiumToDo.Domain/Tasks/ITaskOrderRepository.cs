namespace ProgresiumToDo.Domain.Tasks;

public interface ITaskOrderRepository
{
    void Add(TaskOrder taskOrder);

    void Delete(TaskOrder taskOrder);
    
    void DeleteRange(IEnumerable<TaskOrder> taskOrders);

    Task<TaskOrder> GetByTaskIdAndOrderTypeAsync(Guid taskId, OrderType orderType,
        CancellationToken cancellationToken = default);

    Task<List<TaskOrder>> GetByTaskId(Guid taskId, CancellationToken cancellationToken = default);
    
    Task<decimal> GetMaxOrderIndexByDueDateAsync(DateOnly dueDate, CancellationToken cancellationToken);

    Task<decimal> GetMaxOrderIndexByProjectAsync(Guid projectId, CancellationToken cancellationToken);
}