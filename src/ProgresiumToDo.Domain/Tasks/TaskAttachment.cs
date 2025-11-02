namespace ProgresiumToDo.Domain.Tasks;

public sealed class TaskAttachment
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    
    public string FileUrl { get; private set; }
    
    public string FileName { get; private set; }
    
    public DateTime UploadedAt { get; private set; } = DateTime.UtcNow;
    
    public Guid TaskId { get; private set; }
    
    public TaskItem TaskItem { get; private set;  }
    
    private TaskAttachment(string fileUrl, string fileName, Guid taskId)
    {
        FileUrl = fileUrl;
        FileName = fileName;
        TaskId = taskId;
    }
    
    public static TaskAttachment Create(string fileUrl, string fileName,  Guid taskId)
    {
        return new TaskAttachment(fileUrl, fileName, taskId);
    }
}