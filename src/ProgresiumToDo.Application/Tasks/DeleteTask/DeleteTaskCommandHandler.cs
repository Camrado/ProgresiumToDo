using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.DeleteTask;

internal sealed class DeleteTaskCommandHandler : ICommandHandler<DeleteTaskCommand, DeleteTaskCommandResponse>
{
    private readonly ITaskItemRepository _taskItemRepository;
    
    public DeleteTaskCommandHandler(ITaskItemRepository taskItemRepository)
    {
        _taskItemRepository = taskItemRepository;
    }
    
    public Task<Result<DeleteTaskCommandResponse>> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var taskItem = request.TaskItem!;
        
        _taskItemRepository.Delete(taskItem);
        
        return Task.FromResult<Result<DeleteTaskCommandResponse>>(
            new DeleteTaskCommandResponse("Task deleted successfully."));
    }
}