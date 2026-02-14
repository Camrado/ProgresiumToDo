using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Abstractions.Tags;
using ProgresiumToDo.Application.Abstractions.Tasks;
using ProgresiumToDo.Application.Tasks.Repositories;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.Commands.CreateTask;

internal sealed class CreateTaskCommandHandler : ICommandHandler<CreateTaskCommand, CreateTaskCommandResponse>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly ITaskOrderingService _taskOrderingService;
    private readonly IUserContext _userContext;
    private readonly ITagService _tagService;

    public CreateTaskCommandHandler(
        ITaskItemRepository taskItemRepository,
        ITaskOrderingService taskOrderingService,
        IUserContext userContext,
        ITagService tagService)
    {
        _taskItemRepository = taskItemRepository;
        _taskOrderingService = taskOrderingService;
        _userContext = userContext;
        _tagService = tagService;
    }

    public async Task<Result<CreateTaskCommandResponse>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var taskItem = TaskItem.Create(
            request.ProjectId,
            _userContext.UserId,
            request.Title,
            request.Description,
            request.Status,
            request.Priority,
            request.DueDate,
            request.StartTime,
            request.EndTime);

        if (request.Tags is not null && request.Tags.Count != 0)
        {
            var tags = await _tagService.GetOrCreateTagsAsync(request.Tags, cancellationToken);
            taskItem.SetTags(tags);
        }

        _taskItemRepository.Add(taskItem);

        var orderContext = new TaskOrderContext
        {
            DueDate = request.DueDate,
            ProjectId = request.ProjectId
        };
        await _taskOrderingService.CreateInitialOrdersAsync(taskItem, orderContext, cancellationToken);

        var taskResponse = new CreatedTaskDto(
            taskItem.Id,
            taskItem.ProjectId,
            taskItem.Title,
            taskItem.Description,
            taskItem.Priority.ToString(),
            taskItem.DueDate,
            taskItem.StartTime,
            taskItem.EndTime,
            taskItem.Status.ToString(),
            taskItem.Tags.Select(t => t.Name).ToList(),
            taskItem.CreatedAt);

        return new CreateTaskCommandResponse("CreatedTask created successfully.", taskResponse);
    }
}
