using BarberBoss.Domain.Common;

namespace BarberBoss.Domain.Entities;

public class Company : AuditableEntity
{
    public string Name { get; private set; } = default!;
    private readonly List<Branch> _branches = new();
    public IReadOnlyCollection<Branch> Branches => _branches;

    protected Company() { }

    public Company(string name)
    {
        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name is required") : name.Trim();
    }

    public Branch AddBranch(string name, string? timezone = "America/Sao_Paulo")
    {
        var b = new Branch(Id, name, timezone);
        _branches.Add(b);
        Touch();
        return b;
    }
}
