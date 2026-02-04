using BarberBoss.Application.Abstractions.Payments;
using BarberBoss.Application.Abstractions.Persistence;
using BarberBoss.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BarberBoss.Application.UseCases.Finance.ConfirmPayment;

public class ConfirmPaymentHandler : IRequestHandler<ConfirmPaymentCommand, bool>
{
    private readonly IRepository<Payment> _payments;
    private readonly IPaymentGateway _gateway;
    private readonly IUnitOfWork _uow;

    public ConfirmPaymentHandler(IRepository<Payment> payments, IPaymentGateway gateway, IUnitOfWork uow)
    {
        _payments = payments;
        _gateway = gateway;
        _uow = uow;
    }

    public async Task<bool> Handle(ConfirmPaymentCommand request, CancellationToken ct)
    {
        var payment = await _payments.Query().FirstOrDefaultAsync(p =>
            p.CompanyId == request.CompanyId && p.BranchId == request.BranchId &&
            p.Provider == request.Provider && p.ExternalReference == request.ExternalReference, ct);

        if (payment is null) return false;

        var ok = await _gateway.ConfirmAsync(request.Provider, request.ExternalReference, ct);
        if (!ok) return false;

        payment.MarkPaid(request.ExternalReference);
        _payments.Update(payment);
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}
