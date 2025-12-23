using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Domain.Tasks;

public sealed class TaskItem : BaseEntity
{
    public Guid? ParentTaskItemId { get; private set; }
    
    public string Title { get; private set; }
    
    public string? Description { get; private set; }
    
    public TaskStatus Status { get; private set; }
    
    public Priority? Priority { get; private set; }
    
    public DateOnly? DueDate { get; private set; }
    
    public TimeSpan? Duration { get; private set; }
    
    public TimeOnly? StartTime { get; private set; }
    
    public TimeOnly? EndTime { get; private set; }
    
    public DateTime? ClosedAt { get; private set; }
    
    public decimal OrderIndex { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public Guid ProjectId { get; private set; }
    
    public TaskItem? ParentTaskItem { get; private set; }
    
    public ICollection<TaskItem> SubTaskItems { get; private set; } = new List<TaskItem>();
    
    public User User { get; private set; }
    
    public Project Project { get; private set; }
    
    public ICollection<Tag> Tags { get; private set; } = new List<Tag>();
    
    public ICollection<TaskAttachment> TaskAttachments { get; private set; } = new List<TaskAttachment>();

    private TaskItem() { }

    private TaskItem(
        Guid projectId,
        Guid userId,
        string title,
        string? description,
        TaskStatus status,
        Priority? priority,
        DateOnly? dueDate,
        TimeSpan? duration,
        TimeOnly? startTime,
        TimeOnly? endTime,
        decimal orderIndex)
    {
        ProjectId = projectId;
        UserId = userId;
        Title = title;
        Description = description;
        Status = status;
        Priority = priority;
        DueDate = dueDate;
        Duration = duration;
        StartTime = startTime;
        EndTime = endTime;
        OrderIndex = orderIndex;

        if (startTime.HasValue && duration.HasValue && !endTime.HasValue)
        {
            EndTime = startTime.Value.Add(duration.Value);
        } 
        else if (endTime.HasValue && duration.HasValue && !startTime.HasValue)
        {
            StartTime = endTime.Value.Add(-duration.Value);
        }
        else if (startTime.HasValue && endTime.HasValue && !duration.HasValue)
        {
            Duration = endTime.Value - startTime.Value;
        }
    }

    public static TaskItem Create(
        Guid projectId,
        Guid userId,
        string title,
        string? description,
        string? status,
        string? priority,
        DateOnly? dueDate,
        TimeSpan? duration,
        TimeOnly? startTime,
        TimeOnly? endTime,
        decimal orderIndex)
    {
        var priorityEnum = string.IsNullOrEmpty(priority) 
            ? (Priority?)null 
            : Enum.Parse<Priority>(priority, ignoreCase: true);

        var statusEnum = string.IsNullOrEmpty(status) 
            ? TaskStatus.Pending 
            : Enum.Parse<TaskStatus>(status, ignoreCase: true);

        return new TaskItem(projectId, userId, title, description, statusEnum, priorityEnum, dueDate, duration,
            startTime, endTime, orderIndex);
    }
}