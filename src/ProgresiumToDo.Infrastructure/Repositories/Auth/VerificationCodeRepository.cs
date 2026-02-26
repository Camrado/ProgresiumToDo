using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.Application.Auth.Repositories;
using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Infrastructure.Repositories.Auth;

internal sealed class VerificationCodeRepository : IVerificationCodeRepository
{
    private readonly ApplicationDbContext _dbContext;

    public VerificationCodeRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<VerificationCode?> GetByUserIdAndTypeAsync(
        Guid appUserId, VerificationCodeType type, CancellationToken cancellationToken = default)
    {
        return await _dbContext.VerificationCodes
            .Where(vc => vc.ApplicationUserId == appUserId && vc.Type == type)
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    public void Add(VerificationCode code)
    {
        _dbContext.VerificationCodes.Add(code);
    }
    
    public void Remove(VerificationCode code)
    {
        _dbContext.VerificationCodes.Remove(code);
    }
}
