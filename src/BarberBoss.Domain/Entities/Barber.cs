using BarberBoss.Domain.Common;

namespace BarberBoss.Domain.Entities;

public class Barber : AuditableEntity
{
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }

    public string FullName { get; private set; } = default!;
    public bool Active { get; private set; } = true;
    public decimal Percentage { get; set; }
    public decimal FixedAmount { get; set; }
    public bool Mixed { get; set; }
    public CommissionRule DefaultCommission { get; private set; } = new CommissionRule(0m, 0m);

    protected Barber() { }

    public Barber(Guid companyId, Guid branchId, string fullName, CommissionRule? defaultCommission = null)
    {
        CompanyId = companyId;
        BranchId = branchId;
        FullName = string.IsNullOrWhiteSpace(fullName) ? throw new ArgumentException("FullName is required") : fullName.Trim();
        if (defaultCommission is not null) DefaultCommission = defaultCommission.Value;
    }

    public void UpdateCommission(CommissionRule rule)
    {
        DefaultCommission = rule;
        Touch();
    }
}

public readonly record struct CommissionRule(decimal Percentage, decimal FixedAmount)
{

    public decimal Compute(decimal grossAmount)
        => Math.Round(grossAmount * Percentage + FixedAmount, 2, MidpointRounding.AwayFromZero);
}
