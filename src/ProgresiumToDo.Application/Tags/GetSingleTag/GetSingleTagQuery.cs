using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tags.GetSingleTag;

public sealed record GetSingleTagQuery(Guid TagId) : IQuery<GetSingleTagQueryResponse>
{
    public Guid ProjectId { get; set; }
    
    internal Project? Project { get; set; }
    internal Tag? Tag { get; set; }
}