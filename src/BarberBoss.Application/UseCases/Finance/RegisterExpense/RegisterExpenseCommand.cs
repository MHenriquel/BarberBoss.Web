using MediatR;

namespace BarberBoss.Application.UseCases.Finance.RegisterExpense;

public record RegisterExpenseCommand(Guid CompanyId, Guid BranchId, string Description, decimal Amount, DateTime OccurredAt, string Category) : IRequest<Guid>;
