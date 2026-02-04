using BarberBoss.Domain.Common;

namespace BarberBoss.Domain.Entities;

public class Service : AuditableEntity
{
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }
    public string Name { get; private set; } = default!;
    public int DurationMinutes { get; private set; }
    public decimal Price { get; private set; }
    public CommissionRule? CommissionOverride { get; private set; }

    protected Service() { }

    public Service(Guid companyId, Guid branchId, string name, int durationMinutes, decimal price, CommissionRule? commissionOverride = null)
    {
        CompanyId = companyId;
        BranchId = branchId;
        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name is required") : name.Trim();
        DurationMinutes = durationMinutes <= 0 ? throw new ArgumentException("DurationMinutes must be > 0") : durationMinutes;
        Price = price < 0 ? throw new ArgumentException("Price must be >= 0") : price;
        CommissionOverride = commissionOverride;
    }
}
