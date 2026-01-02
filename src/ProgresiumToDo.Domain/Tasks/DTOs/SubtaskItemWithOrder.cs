namespace ProgresiumToDo.Domain.Tasks.DTOs;

public sealed record SubtaskItemWithOrder(TaskItem SubtaskItem, decimal? OrderIndex);