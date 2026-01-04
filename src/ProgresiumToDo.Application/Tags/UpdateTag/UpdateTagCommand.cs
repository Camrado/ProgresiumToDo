using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tags.UpdateTag;

public sealed record UpdateTagCommand(string? Name, string? Color) : ICommand<UpdateTagCommandResponse>
{
    public Guid TagId { get; set; }
    public Guid ProjectId { get; set; }
    
    internal Tag? Tag { get; set; }
}