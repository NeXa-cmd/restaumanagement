using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RestaurantManager.Web.Pages;

public class SettingsModel : PageModel
{
    public IActionResult OnGet()
    {
        // Check if user is logged in
        if (HttpContext.Session.GetString("IsLoggedIn") != "true")
        {
            return RedirectToPage("/Account/Login");
        }

        return Page();
    }
}
