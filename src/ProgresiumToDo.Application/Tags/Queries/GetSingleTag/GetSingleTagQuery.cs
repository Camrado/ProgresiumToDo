using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Tags;

namespace ProgresiumToDo.Application.Tags.Queries.GetSingleTag;

public sealed record GetSingleTagQuery(Guid TagId) : IQuery<GetSingleTagQueryResponse>
{
    internal Tag? Tag { get; set; }
}