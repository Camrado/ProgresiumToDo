namespace ProgresiumToDo.Domain.Tasks;

public sealed class TaskAttachments
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    
    public string FileUrl { get; private set; }
    
    public string FileName { get; private set; }
    
    public DateTime UploadedAt { get; private set; } = DateTime.UtcNow;
    
    public Guid TaskId { get; private set; }
    
    public Task Task { get; private set;  }
    
    private TaskAttachments(string fileUrl, string fileName, Guid taskId)
    {
        FileUrl = fileUrl;
        FileName = fileName;
        TaskId = taskId;
    }
    
    public static TaskAttachments Create(string fileUrl, string fileName,  Guid taskId)
    {
        return new TaskAttachments(fileUrl, fileName, taskId);
    }
}