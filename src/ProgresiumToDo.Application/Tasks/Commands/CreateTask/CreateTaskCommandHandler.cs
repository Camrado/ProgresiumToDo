using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Abstractions.Tasks;
using ProgresiumToDo.Application.Tags.Repositories;
using ProgresiumToDo.Application.Tasks.Repositories;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Tags.Errors;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.Commands.CreateTask;

internal sealed class CreateTaskCommandHandler : ICommandHandler<CreateTaskCommand, CreateTaskCommandResponse>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly ITagRepository _tagRepository;
    private readonly ITaskOrderingService _taskOrderingService;
    private readonly IUserContext _userContext;

    public CreateTaskCommandHandler(
        ITaskItemRepository taskItemRepository,
        ITagRepository tagRepository,
        ITaskOrderingService taskOrderingService,
        IUserContext userContext)
    {
        _taskItemRepository = taskItemRepository;
        _tagRepository = tagRepository;
        _taskOrderingService = taskOrderingService;
        _userContext = userContext;
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
        
        if (request.TagIds.Count != 0)
        {
            if (!request.ProjectId.HasValue)
            {
                return Result.Failure<CreateTaskCommandResponse>([TagErrors.ProjectIdRequiredForTags]);
            }

            var tags = request.Tags;
            
            if (tags.Count != request.TagIds.Count)
            {
                var foundTagIds = tags.Select(t => t.Id).ToHashSet();
                var missingTagIds = request.TagIds.Where(id => !foundTagIds.Contains(id)).ToList();
                return Result.Failure<CreateTaskCommandResponse>([TagErrors.NotFound(missingTagIds)]);
            }
            
            var invalidTags = tags.Where(t => t.ProjectId != request.ProjectId.Value).ToList();
            if (invalidTags.Count != 0)
            {
                var invalidTagIds = invalidTags.Select(t => t.Id).ToList();
                return Result.Failure<CreateTaskCommandResponse>([TagErrors.NotInProject(invalidTagIds, request.ProjectId.Value)]);
            }
            
            foreach (var tag in tags)
            {
                taskItem.AddTag(tag);
            }
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
            taskItem.CreatedAt);

        return new CreateTaskCommandResponse("CreatedTask created successfully.", taskResponse);
    }
}
