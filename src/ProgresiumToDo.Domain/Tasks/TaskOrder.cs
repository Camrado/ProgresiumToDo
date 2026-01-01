namespace ProgresiumToDo.Domain.Tasks;

public sealed class TaskOrder
{
    public Guid Id { get; protected init; } = Guid.CreateVersion7();
    
    public Guid TaskId { get; private set; }
    
    public OrderType OrderType { get; private set; }
    
    public decimal OrderIndex { get; private set; }
    
    public Guid? ProjectId { get; private set; }
    
    public DateOnly? DueDate { get; private set; }
    
    public Guid? ParentTaskId { get; private set; }
    
    public TaskItem TaskItem { get; private set; }
    
    public Project? Project { get; private set; }

    private TaskOrder()
    {
    }

    private TaskOrder(Guid taskId, OrderType orderType, decimal orderIndex, Guid? projectId, DateOnly? dueDate,
        Guid? parentTaskId)
    {
        TaskId = taskId;
        OrderType = orderType;
        OrderIndex = orderIndex;
        ProjectId = projectId;
        DueDate = dueDate;
        ParentTaskId = parentTaskId;
    }
    
    public static TaskOrder Create(Guid taskId, OrderType orderType, decimal orderIndex, Guid? projectId,
        DateOnly? dueDate, Guid? parentTaskId)
    {
        return new TaskOrder(taskId, orderType, orderIndex, projectId, dueDate, parentTaskId);
    }
    
    public void UpdateOrderIndex(decimal newOrderIndex)
    {
        OrderIndex = newOrderIndex;
    }
    
    public void UpdateProjectId(Guid? newProjectId)
    {
        ProjectId = newProjectId;
    }
    
    public void UpdateDueDate(DateOnly? newDueDate)
    {
        DueDate = newDueDate;
    }
}