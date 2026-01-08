namespace ProgresiumToDo.Domain.Billing;

public interface IPlanRepository
{
    void Add(Plan plan);
    
    Task<Plan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<List<Plan>> GetAllAsync(CancellationToken cancellationToken = default);
}