using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Tags.Queries.GetAllTags;

public sealed record GetAllTagsQuery() : IQuery<GetAllTagsQueryResponse>;