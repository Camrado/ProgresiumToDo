using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Billing.Commands.SubscribeToPlan;
using ProgresiumToDo.Application.Billing.Repositories;
using ProgresiumToDo.Application.Users.Repositories;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth.Errors;

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
        
        var activeSubscription = await _subscriptionRepository.GetActiveSubscriptionByUserIdAsync(
            user.Id, includePlan: true, includeRegion: true, cancellationToken: cancellationToken);

        var userDto = new CurrentUserDto(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.IsEmailVerified,
            user.CreatedAt,
            user.UpdatedAt,
            activeSubscription is not null ? SubscriptionDto.FromDomain(activeSubscription) : null);
        
        return new GetCurrentUserQueryResponse("User profile retrieved successfully.", userDto);
    }
}