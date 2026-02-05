using ProgresiumToDo.Domain.Waitlist;

namespace ProgresiumToDo.Application.Waitlist.Commands.Repositories;

public interface IWaitlistEntryRepository
{
    Task<bool> ExistsAsync(string email, CancellationToken cancellationToken);
    
    void Add(WaitlistEntry waitlistEntry);
}