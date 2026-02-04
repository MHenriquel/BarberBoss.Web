using BarberBoss.Domain.Common;

namespace BarberBoss.Domain.Entities;

public class Customer : AuditableEntity
{
    public Guid CompanyId { get; private set; }
    public string FullName { get; private set; } = default!;
    public string? Phone { get; private set; }
    public string? Email { get; private set; }

    protected Customer() { }

    public Customer(Guid companyId, string fullName, string? phone = null, string? email = null)
    {
        CompanyId = companyId;
        FullName = string.IsNullOrWhiteSpace(fullName) ? throw new ArgumentException("FullName is required") : fullName.Trim();
        Phone = phone;
        Email = email;
    }
}
