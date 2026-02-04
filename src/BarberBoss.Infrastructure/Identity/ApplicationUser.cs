using Microsoft.AspNetCore.Identity;

namespace BarberBoss.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    // Multi-filiais / multi-tenant: guarde CompanyId / BranchId aqui quando evoluir login por barbearia
    public Guid? CompanyId { get; set; }
    public Guid? BranchId { get; set; }

    public string? FullName { get; set; }
}