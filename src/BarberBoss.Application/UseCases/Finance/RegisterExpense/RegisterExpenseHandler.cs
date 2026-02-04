using BarberBoss.Application.Abstractions.Persistence;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.ValueObjects;
using MediatR;

namespace BarberBoss.Application.UseCases.Finance.RegisterExpense;

public class RegisterExpenseHandler : IRequestHandler<RegisterExpenseCommand, Guid>
{
    private readonly IRepository<Expense> _expenses;
    private readonly IUnitOfWork _uow;

    public RegisterExpenseHandler(IRepository<Expense> expenses, IUnitOfWork uow)
    {
        _expenses = expenses;
        _uow = uow;
    }

    public async Task<Guid> Handle(RegisterExpenseCommand request, CancellationToken ct)
    {
        var exp = new Expense(request.CompanyId, request.BranchId, request.Description, new Money(request.Amount), request.OccurredAt, request.Category);
        await _expenses.AddAsync(exp, ct);
        await _uow.SaveChangesAsync(ct);
        return exp.Id;
    }
}
