using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Projects.GetAllProjects;

public sealed record GetAllProjectsQuery : IQuery<GetAllProjectsQueryResponse>
{
    internal IEnumerable<Project>? Projects { get; set; }
};
