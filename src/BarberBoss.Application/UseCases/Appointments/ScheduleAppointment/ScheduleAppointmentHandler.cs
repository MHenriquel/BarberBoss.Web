using BarberBoss.Application.Abstractions.Persistence;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BarberBoss.Application.UseCases.Appointments.ScheduleAppointment;

public class ScheduleAppointmentHandler : IRequestHandler<ScheduleAppointmentCommand, Guid>
{
    private readonly IRepository<Appointment> _appointments;
    private readonly IUnitOfWork _uow;

    public ScheduleAppointmentHandler(IRepository<Appointment> appointments, IUnitOfWork uow)
    {
        _appointments = appointments;
        _uow = uow;
    }

    public async Task<Guid> Handle(ScheduleAppointmentCommand request, CancellationToken ct)
    {
        var overlaps = await _appointments.Query().AnyAsync(a =>
            a.CompanyId == request.CompanyId &&
            a.BranchId == request.BranchId &&
            a.BarberId == request.BarberId &&
            a.Status != AppointmentStatus.Cancelled &&
            request.StartsAt < a.EndsAt && request.EndsAt > a.StartsAt, ct);

        if (overlaps) throw new InvalidOperationException("Horário indisponível para este barbeiro.");

        var appt = new Appointment(request.CompanyId, request.BranchId, request.BarberId, request.CustomerId, request.StartsAt, request.EndsAt, request.Notes);
        foreach (var i in request.Items)
            appt.AddService(i.ServiceId, i.Name, i.UnitPrice, i.Quantity, i.DurationMinutes);

        await _appointments.AddAsync(appt, ct);
        await _uow.SaveChangesAsync(ct);
        return appt.Id;
    }
}
