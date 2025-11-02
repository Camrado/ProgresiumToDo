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
    
    public DateTime? StartTime { get; private set; }
    
    public DateTime? EndTime { get; private set; }
    
    public DateTime? ClosedAt { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public Guid ProjectId { get; private set; }
    
    public TaskItem? ParentTaskItem { get; private set; }
    
    public ICollection<TaskItem> SubTaskItems { get; private set; } = new List<TaskItem>();
    
    public User User { get; private set; }
    
    public Project Project { get; private set; }
    
    public ICollection<Tag> Tags { get; private set; } = new List<Tag>();
    
    public ICollection<TaskAttachment> TaskAttachments { get; private set; } = new List<TaskAttachment>();
}