namespace BarberBoss.Domain.ValueObjects;

public readonly record struct Money(decimal Amount, string Currency = "BRL")
{
    public static Money Zero(string currency = "BRL") => new(0m, currency);
    public Money Add(Money other)
    {
        if (Currency != other.Currency) throw new InvalidOperationException("Currency mismatch");
        return new Money(Amount + other.Amount, Currency);
    }
}
