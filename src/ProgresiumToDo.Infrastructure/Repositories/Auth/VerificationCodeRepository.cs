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

    public async Task AddOrUpdateAsync(Guid appUserId, VerificationCodeType type, string codeHash,
        DateTime expiresOn, CancellationToken cancellationToken = default)
    {
        var existing = await GetByUserIdAndTypeAsync(appUserId, type, cancellationToken);
        if (existing is not null)
        {
            existing.UpdateCode(codeHash, expiresOn);
            return;
        }

        var newCode = type switch
        {
            VerificationCodeType.EmailVerification =>
                VerificationCode.CreateEmailVerificationCode(appUserId, codeHash, expiresOn),
            VerificationCodeType.PasswordReset =>
                VerificationCode.CreatePasswordResetCode(appUserId, codeHash, expiresOn),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown verification code type.")
        };

        try
        {
            Add(newCode);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            // Unique constraint violation — another request inserted first.
            // Detach the failed entity and update the existing one instead.
            _dbContext.Entry(newCode).State = EntityState.Detached;

            var conflicting = await GetByUserIdAndTypeAsync(appUserId, type, cancellationToken);
            conflicting!.UpdateCode(codeHash, expiresOn);
        }
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
