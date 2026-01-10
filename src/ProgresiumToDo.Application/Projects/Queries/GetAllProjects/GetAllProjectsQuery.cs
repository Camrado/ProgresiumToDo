using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Projects.Queries.GetAllProjects;

public sealed record GetAllProjectsQuery() : IQuery<GetAllProjectsQueryResponse>;