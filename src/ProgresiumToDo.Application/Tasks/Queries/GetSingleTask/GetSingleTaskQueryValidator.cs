using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Tasks.Repositories;

namespace ProgresiumToDo.Application.Tasks.Queries.GetSingleTask;

internal sealed class GetSingleTaskQueryValidator : AbstractValidator<GetSingleTaskQuery>
{
    public GetSingleTaskQueryValidator(ITaskItemRepository taskItemRepository, IUserContext userContext)
    {
        RuleFor(gstq => gstq.TaskId)
            .NotEmpty()
            .MustAsync(async (query, taskId, cancellationToken) =>
            {
                var taskItemWithOrder = await taskItemRepository
                    .GetByIdIncludingProjectSubtasksTagsAsync(taskId, userContext.UserId, cancellationToken);
                query.TaskItemWithOrder = taskItemWithOrder;
                
                return taskItemWithOrder is { TaskItem.ParentTaskItemId: null };
            })
            .WithMessage("TaskItemWithOrder not found.");
    }
}