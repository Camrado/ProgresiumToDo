using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Tasks.Commands.AddTagToTask;

internal sealed class AddTagToTaskCommandHandler : ICommandHandler<AddTagToTaskCommand, AddTagToTaskCommandResponse>
{
    public Task<Result<AddTagToTaskCommandResponse>> Handle(AddTagToTaskCommand request, CancellationToken cancellationToken)
    {
        var taskItem = request.TaskItem!;
        var tag = request.Tag!;
        
        taskItem.AddTag(tag);
        
        return Task.FromResult(Result.Success(
            new AddTagToTaskCommandResponse("Tag added to TaskItem successfully.")));
    }
}