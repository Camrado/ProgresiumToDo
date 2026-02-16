using ProgresiumToDo.Application.Abstractions.Auth.Entitlement;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Billing.Repositories;
using ProgresiumToDo.Application.Users.Repositories;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth.Errors;
using ProgresiumToDo.Domain.Billing.Errors;

namespace ProgresiumToDo.Application.Users.Queries.GetCurrentUser;

internal sealed class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, GetCurrentUserQueryResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IEntitlementService _entitlementService;

    public GetCurrentUserQueryHandler(IUserRepository userRepository, IUserContext userContext,
        IEntitlementService entitlementService, ISubscriptionRepository subscriptionRepository)
    {
        _userRepository = userRepository;
        _userContext = userContext;
        _entitlementService = entitlementService;
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<Result<GetCurrentUserQueryResponse>> Handle(GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(_userContext.UserId, cancellationToken: cancellationToken);
        if (user is null)
        {
            return Result.Failure<GetCurrentUserQueryResponse>([UserErrors.UserNotFound]);
        }

        var activeSubscription = await _subscriptionRepository.GetActiveSubscriptionByUserIdAsync(
            user.Id, includePlan: true, includeRegion: true, cancellationToken: cancellationToken);
        if (activeSubscription is null)
        {
            return Result.Failure<GetCurrentUserQueryResponse>([SubscriptionErrors.NoActiveSubscription]);
        }

        var entitlementsResult = await _entitlementService
            .GetUserEntitlementsAsync(user.Id, activeSubscription, cancellationToken);
        if (entitlementsResult.IsFailure)
        {
            return Result.Failure<GetCurrentUserQueryResponse>(entitlementsResult.Errors);
        }

        var subscriptionDetails = SubscriptionDetailsDto.FromDomain(activeSubscription, entitlementsResult.Value);
        var userDto = CurrentUserDto.FromDomain(user, subscriptionDetails);

        return new GetCurrentUserQueryResponse("User profile retrieved successfully.", userDto);
    }
}