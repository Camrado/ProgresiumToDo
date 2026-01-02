using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Tasks;
using ProgresiumToDo.Domain.Tasks.DTOs;

namespace ProgresiumToDo.Application.Tasks.GetSingleTask;

public sealed record GetSingleTaskQuery(Guid TaskId) : IQuery<GetSingleTaskQueryResponse>
{
    internal TaskItemWithOrder? TaskItemWithOrder { get; set; }
}