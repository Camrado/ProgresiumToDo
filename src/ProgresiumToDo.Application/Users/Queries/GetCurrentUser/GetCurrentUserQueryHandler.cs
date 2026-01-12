using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Billing.Commands.SubscribeToPlan;
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
    private readonly IIdentityService _identityService;
    private readonly ISubscriptionRepository _subscriptionRepository;

    public GetCurrentUserQueryHandler(IUserRepository userRepository, IUserContext userContext,
        IIdentityService identityService, ISubscriptionRepository subscriptionRepository)
    {
        _userRepository = userRepository;
        _userContext = userContext;
        _identityService = identityService;
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<Result<GetCurrentUserQueryResponse>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(_userContext.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<GetCurrentUserQueryResponse>([UserErrors.UserNotFound]);
        }
        
        var isEmailVerified = await _identityService.IsEmailVerifiedAsync(user.Email);
        if (isEmailVerified.IsFailure)
        {
            return Result.Failure<GetCurrentUserQueryResponse>(isEmailVerified.Errors);
        }
        
        var activeSubscription = await _subscriptionRepository
            .GetActiveSubscriptionByUserIdAsync(user.Id, includePlan: true, cancellationToken: cancellationToken);
        if (activeSubscription is null)
        {
            return Result.Failure<GetCurrentUserQueryResponse>([SubscriptionErrors.AlreadyOnFreePlan]);
        }
        
        var subscriptionDto = SubscriptionDto.FromDomain(activeSubscription);

        var userDto = new CurrentUserDto(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            isEmailVerified.Value,
            user.CreatedAt,
            user.UpdatedAt,
            subscriptionDto);
        
        return new GetCurrentUserQueryResponse("User profile retrieved successfully.", userDto);
    }
}