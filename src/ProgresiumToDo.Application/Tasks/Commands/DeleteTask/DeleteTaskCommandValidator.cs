using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Tasks.Repositories;

namespace ProgresiumToDo.Application.Tasks.Commands.DeleteTask;

internal sealed class DeleteTaskCommandValidator : AbstractValidator<DeleteTaskCommand>
{
    public DeleteTaskCommandValidator(ITaskItemRepository taskItemRepository, IUserContext userContext)
    {
        RuleFor(dtc => dtc.TaskId)
            .NotEmpty()
            .MustAsync(async (command, taskId, cancellationToken) =>
            {
                var taskItem = await taskItemRepository.GetByIdAsync(taskId, userContext.UserId, trackChanges: true, cancellationToken);
                command.TaskItem = taskItem;

                return taskItem != null;
            })
            .WithMessage("TaskItem not found.");
    }
}