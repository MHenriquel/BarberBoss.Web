using BarberBoss.Application.Abstractions.Payments;
using BarberBoss.Domain.Enums;
using BarberBoss.Domain.ValueObjects;

namespace BarberBoss.Infrastructure.Payments;

public class FakePaymentGateway : IPaymentGateway
{
    public Task<PaymentIntentResult> CreateIntentAsync(Guid companyId, Guid branchId, Guid invoiceId, PaymentMethod method, Money amount, CancellationToken ct = default)
    {
        var ext = $"FAKE-{invoiceId.ToString()[..8]}-{DateTime.UtcNow:yyyyMMddHHmmss}";
        string? qr = null;
        string? url = null;

        if (method == PaymentMethod.Pix)
            qr = $"00020126FAKEPIX{ext}520400005303986540{amount.Amount:0.00}5802BR5909BARBERBOSS6009ITATIBA62{ext}";

        if (method is PaymentMethod.CreditCard or PaymentMethod.DebitCard)
            url = $"https://payments.local/checkout/{ext}";

        return Task.FromResult(new PaymentIntentResult("FakeGateway", ext, method, amount, qr, url));
    }

    public Task<bool> ConfirmAsync(string provider, string externalReference, CancellationToken ct = default)
        => Task.FromResult(true);
}
