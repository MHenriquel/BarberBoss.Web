using MediatR;

namespace BarberBoss.Application.UseCases.Appointments.ScheduleAppointment;

public record ScheduleAppointmentCommand(
    Guid CompanyId,
    Guid BranchId,
    Guid BarberId,
    Guid CustomerId,
    DateTime StartsAt,
    DateTime EndsAt,
    List<ScheduleAppointmentItem> Items,
    string? Notes
) : IRequest<Guid>;

public record ScheduleAppointmentItem(Guid ServiceId, string Name, decimal UnitPrice, int Quantity, int DurationMinutes);
