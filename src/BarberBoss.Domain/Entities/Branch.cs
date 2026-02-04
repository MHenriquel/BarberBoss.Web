using BarberBoss.Domain.Common;

namespace BarberBoss.Domain.Entities;

public class Branch : AuditableEntity
{
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = default!;
    public string Timezone { get; private set; } = "America/Sao_Paulo";
    public string? AddressLine { get; private set; }
    public string? Phone { get; private set; }

    protected Branch() { }

    public Branch(Guid companyId, string name, string? timezone = "America/Sao_Paulo")
    {
        CompanyId = companyId;
        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name is required") : name.Trim();
        Timezone = timezone ?? "America/Sao_Paulo";
    }

    public void UpdateContact(string? addressLine, string? phone)
    {
        AddressLine = addressLine;
        Phone = phone;
        Touch();
    }
}
