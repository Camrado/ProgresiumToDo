using ProgresiumToDo.Application.Abstractions.Auth.Entitlement;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.FeatureUsage;

namespace ProgresiumToDo.Application.Users.Queries.GetEntitlementStatus;

internal sealed class GetEntitlementStatusQueryHandler : IQueryHandler<GetEntitlementStatusQuery, GetEntitlementStatusQueryResponse>
{
    private readonly IUserContext _userContext;
    private readonly IEntitlementService _entitlementService;
    
    public GetEntitlementStatusQueryHandler(IUserContext userContext, IEntitlementService entitlementService)
    {
        _userContext = userContext;
        _entitlementService = entitlementService;
    }
    
    public async Task<Result<GetEntitlementStatusQueryResponse>> Handle(GetEntitlementStatusQuery request, CancellationToken cancellationToken)
    {
        var result = await _entitlementService.GetUserEntitlementsAsync(_userContext.UserId, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure<GetEntitlementStatusQueryResponse>(result.Errors);
        }
        
        var summary = result.Value;

        var dtos = summary.Features
            .Select(fs => MapToDto(fs.Definition, fs.Usage))
            .ToList();

        return new GetEntitlementStatusQueryResponse(
            summary.SubscriptionId,
            summary.PlanName,
            summary.PlanDescription ?? string.Empty,
            summary.CycleStart,
            summary.CycleRenewsAt,
            dtos
        );
    }
    
    private FeatureEntitlementDto MapToDto(PlanFeature pf, FeatureUsageStats stats)
    {
        var isUnlimited = pf.DailyLimit is null && 
                           pf.MonthlyLimit is null && 
                           pf.AbsoluteLimit is null;
        
        return new FeatureEntitlementDto(
            pf.FeatureId,
            pf.Feature.Name.ToString(),
            isUnlimited,
            new QuotaStatus(stats.DailyUsage, pf.DailyLimit),
            new QuotaStatus(stats.MonthlyUsage, pf.MonthlyLimit),
            new QuotaStatus(stats.AbsoluteUsage, pf.AbsoluteLimit)
        );
    }
}