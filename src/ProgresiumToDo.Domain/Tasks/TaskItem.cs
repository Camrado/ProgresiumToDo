using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Domain.Tasks;

public sealed class TaskItem : BaseEntity
{
    public Guid? ParentTaskItemId { get; private set; }
    
    public string Title { get; private set; }
    
    public string? Description { get; private set; }
    
    public TaskStatus Status { get; private set; }

    public Priority Priority { get; private set; }
    
    public DateOnly? DueDate { get; private set; }
    
    public TimeOnly? StartTime { get; private set; }
    
    public TimeOnly? EndTime { get; private set; }
    
    public DateTime? ClosedAt { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public Guid? ProjectId { get; private set; }
    
    public TaskItem? ParentTaskItem { get; private set; }
    
    public ICollection<TaskItem> SubTaskItems { get; private set; } = new List<TaskItem>();
    
    public User User { get; private set; }
    
    public Project? Project { get; private set; }
    
    public ICollection<Tag> Tags { get; private set; } = new List<Tag>();
    
    public ICollection<TaskAttachment> TaskAttachments { get; private set; } = new List<TaskAttachment>();

    private TaskItem() { }

    private TaskItem(
        Guid? projectId,
        Guid userId,
        string title,
        string? description,
        TaskStatus status,
        Priority priority,
        DateOnly? dueDate,
        TimeOnly? startTime,
        TimeOnly? endTime)
    {
        ProjectId = projectId;
        UserId = userId;
        Title = title;
        Description = description;
        Status = status;
        Priority = priority;
        DueDate = dueDate;
        StartTime = startTime;
        EndTime = endTime;
    }

    public static TaskItem Create(
        Guid? projectId,
        Guid userId,
        string title,
        string? description,
        string? status,
        string? priority,
        DateOnly? dueDate,
        TimeOnly? startTime,
        TimeOnly? endTime)
    {
        var priorityEnum = string.IsNullOrEmpty(priority) 
            ? Priority.None
            : Enum.Parse<Priority>(priority, ignoreCase: true);

        var statusEnum = string.IsNullOrEmpty(status) 
            ? TaskStatus.Pending 
            : Enum.Parse<TaskStatus>(status, ignoreCase: true);

        return new TaskItem(projectId, userId, title, description, statusEnum, priorityEnum, dueDate,
            startTime, endTime);
    }
    
    public void Update(
        string? title,
        string? description,
        string? status,
        string? priority,
        DateOnly? dueDate,
        TimeOnly? startTime,
        TimeOnly? endTime,
        Guid? projectId)
    {
        if (title is not null)
            Title = title;
        
        if (description is not null)
            Description = description;
        
        if (status is not null)
            UpdateStatus(Enum.Parse<TaskStatus>(status, ignoreCase: true));
        
        if (priority is not null)
            Priority = Enum.Parse<Priority>(priority, ignoreCase: true);
        
        if (dueDate is not null)
            DueDate = dueDate;
        
        if (startTime is not null)
            StartTime = startTime;
        
        if (endTime is not null)
            EndTime = endTime;
        
        if (projectId is not null)
            ProjectId = projectId.Value;
    }
    
    private void UpdateStatus(TaskStatus newStatus)
    {
        Status = newStatus;
        if (newStatus == TaskStatus.Completed || newStatus == TaskStatus.Cancelled)
        {
            ClosedAt = DateTime.UtcNow;
        }
        else
        {
            ClosedAt = null;
        }
    }
}