using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Projects;

namespace ProgresiumToDo.Application.Projects.Queries.GetProject;

public sealed record GetProjectQuery(Guid ProjectId) : IQuery<GetProjectQueryResponse>
{
    internal Project? Project { get; set; }
};