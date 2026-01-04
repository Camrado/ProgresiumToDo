using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Tags.GetAllTagsForProject;

public sealed record GetAllTagsForProjectQuery() : IQuery<GetAllTagsForProjectQueryResponse>
{
    public Guid ProjectId { get; set; }
}