namespace ProgresiumToDo.Domain.Billing;

public interface IPlanRepository
{
    Task<Plan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<Plan?> GetByIdWithPricingsAndFeaturesIncludedAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<List<Plan>> GetAllAsync(CancellationToken cancellationToken = default);
    
    void Add(Plan plan);
}