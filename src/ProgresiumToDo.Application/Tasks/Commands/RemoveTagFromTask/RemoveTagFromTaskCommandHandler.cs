using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Tasks.Commands.RemoveTagFromTask;

internal sealed class RemoveTagFromTaskCommandHandler : ICommandHandler<RemoveTagFromTaskCommand, RemoveTagFromTaskCommandResponse>
{
    public Task<Result<RemoveTagFromTaskCommandResponse>> Handle(RemoveTagFromTaskCommand request, CancellationToken cancellationToken)
    {
        var taskItem = request.TaskItem!;
        var tag = request.Tag!;
        
        taskItem.RemoveTag(tag);
        
        return Task.FromResult(Result.Success(
            new RemoveTagFromTaskCommandResponse("Tag removed from TaskItem successfully.")));
    }
}