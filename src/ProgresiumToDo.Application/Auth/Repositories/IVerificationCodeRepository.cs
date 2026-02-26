using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Application.Auth.Repositories;

public interface IVerificationCodeRepository
{
    Task<VerificationCode?> GetByUserIdAndTypeAsync(Guid appUserId, VerificationCodeType type, CancellationToken cancellationToken = default);
    void Add(VerificationCode code);
    void Remove(VerificationCode code);
}
