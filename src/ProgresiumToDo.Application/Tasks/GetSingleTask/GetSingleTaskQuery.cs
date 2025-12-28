using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.GetSingleTask;

public sealed record GetSingleTaskQuery(Guid TaskId) : IQuery<GetSingleTaskQueryResponse>
{
    internal TaskItem? TaskItem { get; set; }
}