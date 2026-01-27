using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Application.Abstractions.Auth.Identity;

public interface IIdentityService
{
    Task<Result<Guid>> RegisterUserAsync(string email, string? password = null);

    AuthenticationResult GenerateTokens(User user);
    
    Task<Result<AuthenticationResult>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    
    Task<Result<AuthenticationResult>> RefreshTokensAsync(string oldRefreshTokenValue, CancellationToken cancellationToken = default);
    
    Task<Result> VerifyEmailAsync(string userId, string token);

    Task<Result<string>> GenerateEmailVerificationUrlAsync(string email);

    Task<Result> DeleteAccountAsync(string email);

    Task<Result> AddGoogleLoginAsync(string email, string googleIdentitySub,
        CancellationToken cancellationToken = default);
}