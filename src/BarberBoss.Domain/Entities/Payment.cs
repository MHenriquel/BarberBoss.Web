using BarberBoss.Domain.Common;
using BarberBoss.Domain.Enums;
using BarberBoss.Domain.ValueObjects;

namespace BarberBoss.Domain.Entities;

public class Payment : AuditableEntity
{
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }

    public Guid InvoiceId { get; private set; }
    public Money Amount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;

    public string? Provider { get; private set; }
    public string? ExternalReference { get; private set; }

    protected Payment() { }

    public Payment(Guid companyId, Guid branchId, Guid invoiceId, Money amount, PaymentMethod method, string? provider, string? externalReference)
    {
        CompanyId = companyId;
        BranchId = branchId;
        InvoiceId = invoiceId;
        Amount = amount;
        Method = method;
        Provider = provider;
        ExternalReference = externalReference;
    }

    public void MarkPaid(string? externalReference = null)
    {
        Status = PaymentStatus.Paid;
        ExternalReference = externalReference ?? ExternalReference;
        Touch();
    }
}
