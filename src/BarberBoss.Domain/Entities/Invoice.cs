using BarberBoss.Domain.Common;
using BarberBoss.Domain.ValueObjects;

namespace BarberBoss.Domain.Entities;

public class Invoice : AuditableEntity
{
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }

    public Guid AppointmentId { get; private set; }
    public Guid BarberId { get; private set; }
    public Guid CustomerId { get; private set; }

    public Money Subtotal { get; private set; }
    public Money Discount { get; private set; }
    public Money Total { get; private set; }
    public Money BarberCommission { get; private set; }

    private readonly List<Payment> _payments = new();
    public IReadOnlyCollection<Payment> Payments => _payments;

    protected Invoice() { }

    public Invoice(Guid companyId, Guid branchId, Guid appointmentId, Guid barberId, Guid customerId, Money subtotal, Money discount, Money commission)
    {
        CompanyId = companyId;
        BranchId = branchId;
        AppointmentId = appointmentId;
        BarberId = barberId;
        CustomerId = customerId;
        Subtotal = subtotal;
        Discount = discount;
        Total = new Money(subtotal.Amount - discount.Amount, subtotal.Currency);
        BarberCommission = commission;
    }

    public void AddPayment(Payment p)
    {
        _payments.Add(p);
        Touch();
    }

    public Money PaidAmount() => _payments.Aggregate(Money.Zero(Total.Currency), (acc, p) => acc.Add(p.Amount));
    public bool IsPaid() => PaidAmount().Amount >= Total.Amount;
}
