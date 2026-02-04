//using Microsoft.AspNetCore.Mvc.RazorPages;

//namespace BarberBoss.Web.Pages;

//public class AdminModel : PageModel
//{
//    public void OnGet() { }
//}


using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BarberBoss.Web.Pages;

public class AdminModel : PageModel
{
    // Para demo, você pode “fixar” Company/Branch ou receber via querystring
    public Guid CompanyId { get; private set; }
    public Guid BranchId { get; private set; }

    public void OnGet(Guid? companyId, Guid? branchId)
    {
        // Se você estiver usando o seed, pode copiar os IDs via banco.
        // Para simplificar, deixo valores vazios e você injeta via querystring:
        // /Admin?companyId=...&branchId=...
        CompanyId = companyId ?? Guid.Empty;
        BranchId = branchId ?? Guid.Empty;
    }
}
