using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.DeleteTask;

internal sealed class DeleteTaskCommandValidator : AbstractValidator<DeleteTaskCommand>
{
    public DeleteTaskCommandValidator(ITaskItemRepository taskItemRepository, IUserContext userContext)
    {
        RuleFor(dtc => dtc.TaskId)
            .NotEmpty()
            .MustAsync(async (command, taskId, cancellationToken) =>
            {
                var taskItem = await taskItemRepository.GetByIdAsync(taskId, userContext.UserId, cancellationToken);
                command.TaskItem = taskItem;

                return taskItem != null;
            })
            .WithMessage("TaskItem not found.");
    }
}