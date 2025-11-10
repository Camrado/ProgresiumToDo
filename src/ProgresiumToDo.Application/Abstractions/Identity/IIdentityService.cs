using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Application.Abstractions.Identity;

public interface IIdentityService
{
    Task<Result<Guid>> RegisterAsync(string email, string password);
    
    AuthenticationResult GenerateTokens(User user, CancellationToken cancellationToken = default);
    
    Task<Result<AuthenticationResult>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    
    Task<Result<AuthenticationResult>> RefreshTokensAsync(string oldRefreshTokenValue, CancellationToken cancellationToken = default);
    
    Task<Result> VerifyEmailAsync(string userId, string token);

    Task<Result<string>> GenerateEmailVerificationUrlAsync(string email);

    Task<Result<bool>> IsEmailVerifiedAsync(string email);
}