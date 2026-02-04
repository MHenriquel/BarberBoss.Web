using FluentValidation;

namespace BarberBoss.Application.UseCases.Appointments.ScheduleAppointment;

public class ScheduleAppointmentValidator : AbstractValidator<ScheduleAppointmentCommand>
{
    public ScheduleAppointmentValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.BarberId).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.EndsAt).GreaterThan(x => x.StartsAt);
        RuleFor(x => x.Items).NotEmpty();
    }
}
