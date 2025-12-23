namespace ProgresiumToDo.Application.Tasks.GetTasks;

public sealed record GetTasksQueryResponse(
    string Message,
    IEnumerable<RetrievedTaskResponse> Tasks
    );
    
public sealed record RetrievedTaskResponse(
    Guid Id, 
    string Title, 
    string Priority,
    DateTime? ClosedAt,
    string Status,
    IEnumerable<string> Tags,
    IEnumerable<RetrievedSubTaskResponse> SubTasks,
    string ProjectName,
    DateOnly? DueDate
    );
    
public sealed record RetrievedSubTaskResponse(
    Guid Id,
    string Title,
    string Status
    );