using MediatR;

namespace BarberBoss.Application.UseCases.Appointments.CompleteAppointment;

public record CompleteAppointmentCommand(Guid CompanyId, Guid BranchId, Guid AppointmentId, decimal DiscountAmount) : IRequest<Guid>;
