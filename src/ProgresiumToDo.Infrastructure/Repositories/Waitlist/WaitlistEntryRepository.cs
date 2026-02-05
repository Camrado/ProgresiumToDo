using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.Application.Waitlist.Commands.Repositories;
using ProgresiumToDo.Domain.Waitlist;

namespace ProgresiumToDo.Infrastructure.Repositories.Waitlist;

internal sealed class WaitlistEntryRepository : IWaitlistEntryRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public WaitlistEntryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Task<bool> ExistsAsync(string email, CancellationToken cancellationToken)
    {
        return _dbContext.WaitlistEntries
            .AnyAsync(e => e.Email == email, cancellationToken);
    }

    public void Add(WaitlistEntry waitlistEntry)
    {
        _dbContext.WaitlistEntries.Add(waitlistEntry);
    }
}