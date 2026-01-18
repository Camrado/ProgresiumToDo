using MediatR;
using ProgresiumToDo.Application.Abstractions.Auth.Entitlement;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Abstractions.Behaviors;

public sealed class EntitlementBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IEntitledRequest
{
    private readonly IEntitlementService _entitlementService;
    private readonly IUserContext _userContext;
    
    public EntitlementBehavior(IEntitlementService entitlementService, IUserContext userContext)
    {
        _entitlementService = entitlementService;
        _userContext = userContext;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;
        var requirements = request.GetRequiredEntitlements();
        List<Error> entitlementErrors = new();

        foreach (var featureName in requirements)
        {
            var isAllowed = await _entitlementService.TryIncrementUsageAsync(userId, featureName, cancellationToken);

            if (isAllowed.IsFailure)
            {
                entitlementErrors.AddRange(isAllowed.Errors);
            }
        }
        
        if (entitlementErrors.Any())
        {
            throw new EntitlementException(entitlementErrors);
        }
        
        return await next(cancellationToken);
    }
}