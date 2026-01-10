using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Abstractions.Onboarding;

public interface IUserOnboardingService
{
    Task<Result<AuthenticationResult>> RegisterAndOnboardUserAsync( 
        string email,
        string password,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default);
}