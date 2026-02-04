using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BarberBoss.Web.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        if (User?.Identity?.IsAuthenticated == true)
            return RedirectToPage("/Admin");

        return RedirectToPage("/Account/Login");
    }
}