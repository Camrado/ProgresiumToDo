using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tags.DeleteTag;

public sealed record DeleteTagCommand(Guid TagId) : ICommand<DeleteTagCommandResponse>
{
    public Guid ProjectId { get; set; }
    
    internal Tag? Tag { get; set; }
}