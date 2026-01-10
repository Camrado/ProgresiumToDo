using FluentValidation;
using ProgresiumToDo.Application.Billing.Repositories;

namespace ProgresiumToDo.Application.Billing.Queries.GetSinglePlan;

internal sealed class GetSinglePlanQueryValidator : AbstractValidator<GetSinglePlanQuery>
{
    public GetSinglePlanQueryValidator(IPlanRepository planRepository)
    {
        RuleFor(gspq => gspq.PlanId)
            .NotEmpty()
            .WithMessage("Plan ID must not be empty.")
            .MustAsync(async (query, planId, cancellationToken) =>
            {
                var plan = await planRepository.GetByIdWithPricingsAndFeaturesIncludedAsync(planId, cancellationToken);
                query.Plan = plan;
                
                return plan is not null;
            })
            .WithMessage("Plan with the specified ID does not exist.");
    }
}