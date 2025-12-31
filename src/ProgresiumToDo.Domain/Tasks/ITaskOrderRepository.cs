namespace ProgresiumToDo.Domain.Tasks;

public interface ITaskOrderRepository
{
    void Add(TaskOrder taskOrder);

    Task<decimal?> GetNextOrderIndexAsync(OrderType orderType, Guid? projectId, DateOnly? dueDate, Guid? parentTaskId, 
        CancellationToken cancellationToken = default);
}