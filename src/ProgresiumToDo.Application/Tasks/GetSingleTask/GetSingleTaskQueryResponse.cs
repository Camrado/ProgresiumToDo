namespace ProgresiumToDo.Application.Tasks.GetSingleTask;

public sealed record GetSingleTaskQueryResponse(
    string Message,
    TaskDetailsDto Task);
    
public sealed record TaskDetailsDto();