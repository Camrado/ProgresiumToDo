using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Waitlist.Commands.Repositories;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Waitlist;

namespace ProgresiumToDo.Application.Waitlist.Commands.JoinWaitlist;

internal sealed class JoinWaitlistCommandHandler : ICommandHandler<JoinWaitlistCommand, JoinWaitlistCommandResponse>
{
    private readonly IWaitlistEntryRepository _waitlistEntryRepository;

    public JoinWaitlistCommandHandler(IWaitlistEntryRepository waitlistEntryRepository)
    {
        _waitlistEntryRepository = waitlistEntryRepository;
    } 
    
    public async Task<Result<JoinWaitlistCommandResponse>> Handle(JoinWaitlistCommand request, CancellationToken cancellationToken)
    {
        var emailExistsInWaitlist = await _waitlistEntryRepository.ExistsAsync(request.Email, cancellationToken);
        if (!emailExistsInWaitlist)
        {
            var waitlistEntry = WaitlistEntry.Create(request.Email);
            _waitlistEntryRepository.Add(waitlistEntry);
        }
        
        return new JoinWaitlistCommandResponse("You've successfully joined the waitlist!");
    }
}