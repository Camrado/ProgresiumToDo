namespace ProgresiumToDo.Domain.Tasks.DTOs;

public record TaskItemWithOrder(TaskItem? TaskItem, decimal? OrderIndex, IReadOnlyList<SubtaskItemWithOrder>? Subtasks);