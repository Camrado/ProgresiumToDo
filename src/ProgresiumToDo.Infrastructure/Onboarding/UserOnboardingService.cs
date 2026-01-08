using ProgresiumToDo.Application.Abstractions.Billing;
using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Application.Abstractions.Onboarding;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Infrastructure.Onboarding;

internal sealed class UserOnboardingService : IUserOnboardingService
{
    private readonly IIdentityService _identityService;
    private readonly IUserRepository _userRepository;
    private readonly ISubscriptionService _subscriptionService;
    private readonly ApplicationDbContext _dbContext;
    
    public UserOnboardingService(IIdentityService identityService, IUserRepository userRepository,
        ISubscriptionService subscriptionService, ApplicationDbContext dbContext)
    {
        _identityService = identityService;
        _userRepository = userRepository;
        _subscriptionService = subscriptionService;
        _dbContext = dbContext;
    }

    public async Task<Result<AuthenticationResult>> RegisterAndOnboardUserAsync(string email,
        string password, string firstName, string lastName, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var authenticationResult = await _identityService.RegisterAsync(email, password);
            if (authenticationResult.IsFailure)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result.Failure<AuthenticationResult>(authenticationResult.Errors);
            }

            var user = User.Create(email, firstName, lastName, authenticationResult.Value);
            _userRepository.Add(user);

            var subscriptionResult = await _subscriptionService.SubscribeUserToFreePlanAsync(user.Id, cancellationToken);
            if (subscriptionResult.IsFailure)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result.Failure<AuthenticationResult>(subscriptionResult.Errors);
            }

            await transaction.CommitAsync(cancellationToken);

            var tokens = _identityService.GenerateTokens(user);

            return Result.Success(tokens);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}