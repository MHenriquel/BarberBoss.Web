using BarberBoss.Domain.Common;
using BarberBoss.Domain.Enums;

namespace BarberBoss.Domain.Entities;

public class Appointment : AuditableEntity
{
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid BarberId { get; private set; }
    public Guid CustomerId { get; private set; }

    public DateTime StartsAt { get; private set; }
    public DateTime EndsAt { get; private set; }

    public AppointmentStatus Status { get; private set; } = AppointmentStatus.Scheduled;
    public string? Notes { get; private set; }

    private readonly List<AppointmentItem> _items = new();
    public IReadOnlyCollection<AppointmentItem> Items => _items;

    protected Appointment() { }

    public Appointment(Guid companyId, Guid branchId, Guid barberId, Guid customerId, DateTime startsAt, DateTime endsAt, string? notes = null)
    {
        if (endsAt <= startsAt) throw new ArgumentException("EndsAt must be greater than StartsAt");
        CompanyId = companyId;
        BranchId = branchId;
        BarberId = barberId;
        CustomerId = customerId;
        StartsAt = startsAt;
        EndsAt = endsAt;
        Notes = notes;
    }

    public void AddService(Guid serviceId, string name, decimal unitPrice, int qty = 1, int durationMinutes = 30)
    {
        if (qty <= 0) throw new ArgumentException("qty must be > 0");
        _items.Add(new AppointmentItem(serviceId, name, unitPrice, qty, durationMinutes));
        Touch();
    }

    public void Complete()
    {
        if (Status == AppointmentStatus.Cancelled) throw new InvalidOperationException("Cancelled cannot be completed");
        Status = AppointmentStatus.Completed;
        Touch();
    }
}

public class AppointmentItem
{
    public Guid ServiceId { get; private set; }
    public string Name { get; private set; } = default!;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public int DurationMinutes { get; private set; }

    protected AppointmentItem() { }

    public AppointmentItem(Guid serviceId, string name, decimal unitPrice, int quantity, int durationMinutes)
    {
        ServiceId = serviceId;
        Name = name;
        UnitPrice = unitPrice;
        Quantity = quantity;
        DurationMinutes = durationMinutes;
    }

    public decimal Total() => UnitPrice * Quantity;
}
