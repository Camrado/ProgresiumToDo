using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Projects;
using ProgresiumToDo.Domain.Tags;

namespace ProgresiumToDo.Application.Tags.Queries.GetSingleTag;

public sealed record GetSingleTagQuery(Guid TagId) : IQuery<GetSingleTagQueryResponse>
{
    public Guid ProjectId { get; set; }
    
    internal Project? Project { get; set; }
    internal Tag? Tag { get; set; }
}