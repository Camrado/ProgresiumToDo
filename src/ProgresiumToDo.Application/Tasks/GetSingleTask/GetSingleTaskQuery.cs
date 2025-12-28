using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Tasks.GetSingleTask;

public sealed record GetSingleTaskQuery(Guid TaskId) : IQuery<GetSingleTaskQueryResponse>;