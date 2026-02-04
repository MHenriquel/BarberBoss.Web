using MediatR;

namespace BarberBoss.Application.UseCases.Finance.ConfirmPayment;

public record ConfirmPaymentCommand(Guid CompanyId, Guid BranchId, string Provider, string ExternalReference) : IRequest<bool>;
