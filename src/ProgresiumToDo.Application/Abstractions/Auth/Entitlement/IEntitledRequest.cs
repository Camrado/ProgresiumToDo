using ProgresiumToDo.Domain.FeatureUsage;

namespace ProgresiumToDo.Application.Abstractions.Auth.Entitlement;

public interface IEntitledRequest
{
    IEnumerable<FeatureName> GetRequirements();
}