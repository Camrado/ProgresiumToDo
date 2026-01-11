using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Auth.Onboarding;
using ProgresiumToDo.Application.Abstractions.Billing;
using ProgresiumToDo.Application.Users.Repositories;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Infrastructure.Auth.Onboarding;

internal sealed class UserOnboardingService : IUserOnboardingService
{
    private readonly IIdentityService _identityService;
    private readonly IUserRepository _userRepository;
    private readonly ISubscriptionService _subscriptionService;
    
    public UserOnboardingService(IIdentityService identityService, IUserRepository userRepository,
        ISubscriptionService subscriptionService)
    {
        _identityService = identityService;
        _userRepository = userRepository;
        _subscriptionService = subscriptionService;
    }

    public async Task<Result<AuthenticationResult>> RegisterAndOnboardUserAsync(string email,
        string password, string firstName, string lastName, CancellationToken cancellationToken = default)
    {
        var authenticationResult = await _identityService.RegisterUserAsync(email, password);
        if (authenticationResult.IsFailure)
        {
            return Result.Failure<AuthenticationResult>(authenticationResult.Errors);
        }

        var user = User.Create(email, firstName, lastName, authenticationResult.Value);
        _userRepository.Add(user);

        var subscriptionResult = await _subscriptionService.SubscribeUserToFreePlanAsync(user.Id, cancellationToken);
        if (subscriptionResult.IsFailure)
        {
            return Result.Failure<AuthenticationResult>(subscriptionResult.Errors);
        }

        var tokens = _identityService.GenerateTokens(user);

        return Result.Success(tokens);
    }
}