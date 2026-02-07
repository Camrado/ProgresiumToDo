using ProgresiumToDo.Domain.Waitlist;

namespace ProgresiumToDo.Application.Waitlist.Repositories;

public interface IWaitlistEntryRepository
{
    Task<bool> ExistsAsync(string email, CancellationToken cancellationToken);
    
    void Add(WaitlistEntry waitlistEntry);
}