using BarberBoss.Domain.Enums;
using BarberBoss.Domain.ValueObjects;

namespace BarberBoss.Application.Abstractions.Payments;

public record PaymentIntentResult(string Provider, string ExternalReference, PaymentMethod Method, Money Amount, string? PixQrCode, string? RedirectUrl);

public interface IPaymentGateway
{
    Task<PaymentIntentResult> CreateIntentAsync(Guid companyId, Guid branchId, Guid invoiceId, PaymentMethod method, Money amount, CancellationToken ct = default);
    Task<bool> ConfirmAsync(string provider, string externalReference, CancellationToken ct = default);
}
