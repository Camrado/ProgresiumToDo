using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Abstractions.Auth.Onboarding;

public interface IUserOnboardingService
{
    Task<Result<AuthenticationResult>> RegisterAndOnboardUserAsync( 
        string email,
        string password,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default);
}