using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Projects.GetAllProjects;

public sealed record GetAllProjectsQuery() : IQuery<GetAllProjectsQueryResponse>;