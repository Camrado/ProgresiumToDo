using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Tasks.Repositories.DTOs;

namespace ProgresiumToDo.Application.Tasks.Queries.GetSingleTask;

public sealed record GetSingleTaskQuery(Guid TaskId) : IQuery<GetSingleTaskQueryResponse>
{
    internal TaskItemWithOrder? TaskItemWithOrder { get; set; }
}