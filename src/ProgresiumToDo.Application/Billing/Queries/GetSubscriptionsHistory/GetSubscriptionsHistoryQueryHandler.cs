using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Billing.Commands.SubscribeToPlan;
using ProgresiumToDo.Application.Billing.Repositories;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Billing.Queries.GetSubscriptionsHistory;

internal sealed class GetSubscriptionsHistoryQueryHandler : IQueryHandler<GetSubscriptionsHistoryQuery, GetSubscriptionsHistoryQueryResponse>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserContext _userContext;
    
    public GetSubscriptionsHistoryQueryHandler(ISubscriptionRepository subscriptionRepository, IUserContext userContext)
    {
        _subscriptionRepository = subscriptionRepository;
        _userContext = userContext;
    }
    
    public async Task<Result<GetSubscriptionsHistoryQueryResponse>> Handle(GetSubscriptionsHistoryQuery request, CancellationToken cancellationToken)
    {
        var subscriptions = await _subscriptionRepository
            .GetPaidSubscriptionsByUserIdAsync(_userContext.UserId, includePlan: true, cancellationToken: cancellationToken);

        var subscriptionDtos = subscriptions
            .Select(SubscriptionDto.FromDomain);
        
        return new GetSubscriptionsHistoryQueryResponse(
            "Subscriptions history retrieved successfully.", subscriptionDtos);
    }
}