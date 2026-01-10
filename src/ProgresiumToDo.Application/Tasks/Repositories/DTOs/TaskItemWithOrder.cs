using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.Repositories.DTOs;

public record TaskItemWithOrder(TaskItem? TaskItem, decimal? OrderIndex, IReadOnlyList<SubtaskItemWithOrder>? Subtasks);