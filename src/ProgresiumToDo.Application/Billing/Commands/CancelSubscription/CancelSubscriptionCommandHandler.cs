using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Billing;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Billing.Commands.CancelSubscription;

internal sealed class CancelSubscriptionCommandHandler : ICommandHandler<CancelSubscriptionCommand, CancelSubscriptionCommandResponse>
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IUserContext _userContext;
    
    public CancelSubscriptionCommandHandler(ISubscriptionService subscriptionService, IUserContext userContext)
    {
        _subscriptionService = subscriptionService;
        _userContext = userContext;
    }
    
    public async Task<Result<CancelSubscriptionCommandResponse>> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var cancellationResult =
            await _subscriptionService.CancelUserSubscriptionAsync(_userContext.UserId, cancellationToken);

        if (cancellationResult.IsFailure)
        {
            return Result.Failure<CancelSubscriptionCommandResponse>(cancellationResult.Errors);
        }
        
        return new CancelSubscriptionCommandResponse("Subscription cancelled successfully.");
    }
}