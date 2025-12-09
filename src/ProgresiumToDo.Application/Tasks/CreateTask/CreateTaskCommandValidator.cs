using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Domain.Tasks;
using TaskStatus = ProgresiumToDo.Domain.Tasks.TaskStatus;

namespace ProgresiumToDo.Application.Tasks.CreateTask;

internal sealed class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserContext _userContext;

    public CreateTaskCommandValidator(IProjectRepository projectRepository, IUserContext userContext)
    {
        _projectRepository = projectRepository;
        _userContext = userContext;

        RuleFor(ctc => ctc.Title)
            .NotEmpty()
            .WithMessage("Title is required.");

        RuleFor(ctc => ctc.ProjectId)
            .NotEmpty()
            .MustAsync(async (command, projectId, cancellationToken) =>
            {
                var project = await _projectRepository.GetByIdAndUserIdAsync(projectId, _userContext.UserId, cancellationToken);
                return project != null;
            }).WithMessage("Project not found.");

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
                    return false;

                if (command.StartTime.HasValue && command.EndTime.HasValue)
                    return command.EndTime.Value > command.StartTime.Value;

                return true;
            }).WithMessage("Invalid time configuration. If endTime is provided, startTime must also be provided and endTime must be after startTime.");
    }
}
