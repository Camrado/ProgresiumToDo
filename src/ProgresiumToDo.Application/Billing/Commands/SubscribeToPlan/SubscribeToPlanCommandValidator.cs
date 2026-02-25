using FluentValidation;
using ProgresiumToDo.Application.Billing.Repositories;

namespace ProgresiumToDo.Application.Billing.Commands.SubscribeToPlan;

internal sealed class SubscribeToPlanCommandValidator : AbstractValidator<SubscribeToPlanCommand>
{
    public SubscribeToPlanCommandValidator(IPlanPricingRepository planPricingRepository)
    {
        RuleFor(stp => stp.PlanPricingId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("PlanPricing ID must not be empty.")
            .MustAsync(async (command, planPricingId, cancellationToken) =>
            {
                var planPricing = await planPricingRepository.GetByIdAsync(
                    planPricingId, includePlan: true, includeRegion: true, trackChanges: true, cancellationToken: cancellationToken);
                if (planPricing is null)
                    return false;
                
                command.PlanPricing = planPricing;
                return true;
            })
            .WithMessage("The specified plan pricing does not exist.");
        
        RuleFor(stp => stp.IsAutoRenew)
            .NotEmpty()
            .WithMessage("Auto-renewal preference must be specified.");
    }
}