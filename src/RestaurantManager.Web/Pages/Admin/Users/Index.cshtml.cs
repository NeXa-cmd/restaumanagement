using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Domain.Entities;
using RestaurantManager.Infrastructure.Persistence;

namespace RestaurantManager.Web.Pages.Admin.Users;

public class IndexModel : PageModel
{
    private readonly RestaurantDbContext _context;

    public IndexModel(RestaurantDbContext context)
    {
        _context = context;
    }

    public List<User> Users { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetString("IsLoggedIn") != "true")
        {
            return RedirectToPage("/Account/Login");
        }

        if (HttpContext.Session.GetString("UserRole") != "Admin")
        {
            return RedirectToPage("/Index");
        }

        Users = await _context.Users
            .Include(u => u.Orders)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        return Page();
    }
}
