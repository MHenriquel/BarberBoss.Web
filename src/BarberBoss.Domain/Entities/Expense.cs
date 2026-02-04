using BarberBoss.Domain.Common;
using BarberBoss.Domain.ValueObjects;

namespace BarberBoss.Domain.Entities;

public class Expense : AuditableEntity
{
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }

    public string Description { get; private set; } = default!;
    public Money Amount { get; private set; }
    public DateTime OccurredAt { get; private set; }
    public string Category { get; private set; } = "General";

    protected Expense() { }

    public Expense(Guid companyId, Guid branchId, string description, Money amount, DateTime occurredAt, string category)
    {
        CompanyId = companyId;
        BranchId = branchId;
        Description = description;
        Amount = amount;
        OccurredAt = occurredAt;
        Category = category;
    }
}
