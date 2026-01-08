namespace ProgresiumToDo.Domain.Billing;

public interface IPlanRepository
{
    Task<Plan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Plan?> GeyByNameWithPricingsIncludedAsync(string name, CancellationToken cancellationToken = default);
    
    Task<Plan?> GetByIdWithPricingsAndFeaturesIncludedAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<List<Plan>> GetAllWithPricingsAndFeaturesIncludedAsync(CancellationToken cancellationToken = default);
    
    void Add(Plan plan);
}