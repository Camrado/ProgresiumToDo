using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.Repositories.DTOs;

public sealed record SubtaskItemWithOrder(TaskItem SubtaskItem, decimal? OrderIndex);