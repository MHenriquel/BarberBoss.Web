using BarberBoss.Application.Abstractions.Persistence;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BarberBoss.Application.UseCases.Appointments.CompleteAppointment;

public class CompleteAppointmentHandler : IRequestHandler<CompleteAppointmentCommand, Guid>
{
    private readonly IRepository<Appointment> _appointments;
    private readonly IRepository<Service> _services;
    private readonly IRepository<Barber> _barbers;
    private readonly IRepository<Invoice> _invoices;
    private readonly IUnitOfWork _uow;

    public CompleteAppointmentHandler(IRepository<Appointment> appointments, IRepository<Service> services, IRepository<Barber> barbers, IRepository<Invoice> invoices, IUnitOfWork uow)
    {
        _appointments = appointments;
        _services = services;
        _barbers = barbers;
        _invoices = invoices;
        _uow = uow;
    }

    public async Task<Guid> Handle(CompleteAppointmentCommand request, CancellationToken ct)
    {
        var appt = await _appointments.Query().Include(a => a.Items)
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId && a.CompanyId == request.CompanyId && a.BranchId == request.BranchId, ct)
            ?? throw new KeyNotFoundException("Agendamento não encontrado.");

        appt.Complete();

        var barber = await _barbers.GetByIdAsync(appt.BarberId, ct) ?? throw new KeyNotFoundException("Barbeiro não encontrado");

        var serviceIds = appt.Items.Select(i => i.ServiceId).Distinct().ToList();
        var svcMap = await _services.Query().Where(s => serviceIds.Contains(s.Id)).ToDictionaryAsync(s => s.Id, ct);

        decimal subtotal = 0m;
        decimal commission = 0m;
        foreach (var item in appt.Items)
        {
            subtotal += item.Total();
            var svc = svcMap[item.ServiceId];
            var rule = svc.CommissionOverride ?? barber.DefaultCommission;
            commission += rule.Compute(item.Total());
        }

        var discount = Math.Clamp(request.DiscountAmount, 0m, subtotal);

        var invoice = new Invoice(appt.CompanyId, appt.BranchId, appt.Id, appt.BarberId, appt.CustomerId,
            new Money(subtotal), new Money(discount), new Money(commission));

        await _invoices.AddAsync(invoice, ct);
        await _uow.SaveChangesAsync(ct);

        return invoice.Id;
    }
}
