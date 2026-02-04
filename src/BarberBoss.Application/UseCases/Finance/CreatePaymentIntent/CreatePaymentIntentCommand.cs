using BarberBoss.Domain.Enums;
using MediatR;

namespace BarberBoss.Application.UseCases.Finance.CreatePaymentIntent;

public record CreatePaymentIntentCommand(Guid CompanyId, Guid BranchId, Guid InvoiceId, PaymentMethod Method) : IRequest<CreatePaymentIntentResultDto>;

public record CreatePaymentIntentResultDto(string Provider, string ExternalReference, string Method, decimal Amount, string Currency, string? PixQrCode, string? RedirectUrl);
