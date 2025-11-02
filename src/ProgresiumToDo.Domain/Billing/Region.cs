using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Domain.Billing;

public sealed class Region : BaseEntity
{
    public string Code { get; private set; }
    
    public string Name { get; private set; }
    
    public string Currency { get; private set; }
    
    public ICollection<PlanPricing> PlanPricings { get; private set; } = new List<PlanPricing>();
    
    private Region(string code, string name, string currency)
    {
        Code = code;
        Name = name;
        Currency = currency;
    }
    
    public static Region Create(string code, string name, string currency)
    {
        return new Region(code, name, currency);
    }
}