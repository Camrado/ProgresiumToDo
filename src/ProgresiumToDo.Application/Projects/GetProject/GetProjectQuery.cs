using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Projects.GetProject;

public sealed record GetProjectQuery(Guid ProjectId) : IQuery<GetProjectQueryResponse>
{
    internal Project? Project { get; set; }
};