using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Projects.Repositories;
using ProgresiumToDo.Application.Tags.Repositories;
using ProgresiumToDo.Domain.Tasks;
using TaskStatus = ProgresiumToDo.Domain.Tasks.TaskStatus;

namespace ProgresiumToDo.Application.Tasks.Commands.CreateTask;

internal sealed class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator(IProjectRepository projectRepository, ITagRepository tagRepository, IUserContext userContext)
    {
        RuleFor(ctc => ctc.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(256)
            .WithMessage("Title must not exceed 256 characters.");;

        RuleFor(ctc => ctc.ProjectId)
            .Cascade(CascadeMode.Stop)
            .MustAsync(async (command, projectId, cancellationToken) =>
            {
                if (!projectId.HasValue)
                    return true;
                
                var project = await projectRepository.GetByIdAndUserIdAsync(projectId.Value, userContext.UserId, cancellationToken);
                return project != null;
            }).WithMessage("Project not found.")
            .DependentRules(() =>
            {
                RuleForEach(ctc => ctc.TagIds)
                    .MustAsync(async (command, tagId, cancellationToken) =>
                    {
                        var tag = await tagRepository.GetByIdAndProjectIdAsync(tagId, (Guid)command.ProjectId!, cancellationToken);
                        
                        if (tag is not null) 
                        {
                            command.Tags.Add(tag);
                            return true;
                        }
                        return false;
                    })
                    .WithMessage("Tag {PropertyValue} not found in the specified project.");
            });

        RuleFor(ctc => ctc.Priority)
            .Must(priority => string.IsNullOrEmpty(priority) || Enum.TryParse<Priority>(priority, ignoreCase: true, out _))
            .WithMessage("Invalid priority. Valid values are: none, low, medium, high.");

        RuleFor(ctc => ctc.Status)
            .Must(status => string.IsNullOrEmpty(status) || Enum.TryParse<TaskStatus>(status, ignoreCase: true, out _))
            .WithMessage("Invalid status. Valid values are: pending, inprogress, completed, cancelled.");

        RuleFor(ctc => ctc)
            .Must(command =>
            {
                if (command.EndTime.HasValue && !command.StartTime.HasValue)
                    return true;
                
                if (!command.EndTime.HasValue && command.StartTime.HasValue)
                    return true;

                if (command.StartTime.HasValue && command.EndTime.HasValue)
                    return command.EndTime.Value > command.StartTime.Value;

                return true;
            }).WithMessage("Invalid time configuration. If endTime is provided, startTime must also be provided and endTime must be after startTime.");
    }
}
