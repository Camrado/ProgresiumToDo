using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.RemoveTagFromTask;

internal sealed class RemoveTagFromTaskCommandValidator : AbstractValidator<RemoveTagFromTaskCommand>
{
    public RemoveTagFromTaskCommandValidator(ITaskItemRepository taskItemRepository, IUserContext userContext, ITagRepository tagRepository)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(att => att.TaskId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("TaskId must not be null.")
            .MustAsync(async (command, taskId, cancellationToken) =>
            {
                var taskItem = await taskItemRepository.GetByIdWithTagsIncludedAsync(taskId, userContext.UserId, cancellationToken);
                command.TaskItem = taskItem;

                return taskItem != null;
            })
            .WithMessage("TaskItem not found.")
            .Must((command, taskId) => command.TaskItem!.ProjectId is not null)
            .WithMessage("TaskItem must be associated with a project.");

        RuleFor(att => att.TagId)
            .NotNull()
            .WithMessage("TagId must not be null.")
            .MustAsync(async (command, tagId, cancellationToken) =>
            {
                var tag = await tagRepository.GetByIdAndProjectIdAsync(tagId, (Guid)command.TaskItem!.ProjectId!, cancellationToken);
                command.Tag = tag;
                
                return tag is not null;
            })
            .WithMessage("Tag not found in the associated project.");
    }
}