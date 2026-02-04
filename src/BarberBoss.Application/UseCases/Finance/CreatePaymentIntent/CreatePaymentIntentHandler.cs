using BarberBoss.Application.Abstractions.Payments;
using BarberBoss.Application.Abstractions.Persistence;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BarberBoss.Application.UseCases.Finance.CreatePaymentIntent;

public class CreatePaymentIntentHandler : IRequestHandler<CreatePaymentIntentCommand, CreatePaymentIntentResultDto>
{
    private readonly IRepository<Invoice> _invoices;
    private readonly IRepository<Payment> _payments;
    private readonly IPaymentGateway _gateway;
    private readonly IUnitOfWork _uow;

    public CreatePaymentIntentHandler(IRepository<Invoice> invoices, IRepository<Payment> payments, IPaymentGateway gateway, IUnitOfWork uow)
    {
        _invoices = invoices;
        _payments = payments;
        _gateway = gateway;
        _uow = uow;
    }

    public async Task<CreatePaymentIntentResultDto> Handle(CreatePaymentIntentCommand request, CancellationToken ct)
    {
        var invoice = await _invoices.Query().Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId && i.CompanyId == request.CompanyId && i.BranchId == request.BranchId, ct)
            ?? throw new KeyNotFoundException("Invoice não encontrada");

        if (invoice.IsPaid()) throw new InvalidOperationException("Invoice já paga.");

        var remaining = invoice.Total.Amount - invoice.PaidAmount().Amount;
        var money = new Money(remaining, invoice.Total.Currency);

        var intent = await _gateway.CreateIntentAsync(request.CompanyId, request.BranchId, invoice.Id, request.Method, money, ct);

        var payment = new Payment(request.CompanyId, request.BranchId, invoice.Id, money, request.Method, intent.Provider, intent.ExternalReference);
        invoice.AddPayment(payment);

        await _payments.AddAsync(payment, ct);
        _invoices.Update(invoice);
        await _uow.SaveChangesAsync(ct);

        return new CreatePaymentIntentResultDto(intent.Provider, intent.ExternalReference, request.Method.ToString(), intent.Amount.Amount, intent.Amount.Currency, intent.PixQrCode, intent.RedirectUrl);
    }
}
