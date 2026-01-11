using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Billing;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Billing.Commands.SubscribeToPlan;

internal sealed class SubscribeToPlanCommandHandler : ICommandHandler<SubscribeToPlanCommand, SubscribeToPlanCommandResponse>
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IUserContext _userContext;
    
    public SubscribeToPlanCommandHandler(ISubscriptionService subscriptionService, IUserContext userContext)
    {
        _subscriptionService = subscriptionService;
        _userContext = userContext;
    }
    
    public async Task<Result<SubscribeToPlanCommandResponse>> Handle(SubscribeToPlanCommand request, CancellationToken cancellationToken)
    {
        var subscriptionResult = await _subscriptionService.SubscribeUserToPlanAsync(
            _userContext.UserId,
            request.PlanPricing,
            request.IsAutoRenew,
            cancellationToken);

        if (subscriptionResult.IsFailure)
        {
            return Result.Failure<SubscribeToPlanCommandResponse>(subscriptionResult.Errors);
        }
        
        var subscriptionDto = SubscriptionDto.FromDomain(subscriptionResult.Value);
        
        return new SubscribeToPlanCommandResponse("Subscription successful.", subscriptionDto);
    }
}