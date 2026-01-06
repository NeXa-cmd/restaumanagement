using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Domain.Entities;
using RestaurantManager.Infrastructure.Persistence;

namespace RestaurantManager.Web.Pages.Admin;

public class IndexModel : PageModel
{
    private readonly RestaurantDbContext _context;

    public IndexModel(RestaurantDbContext context)
    {
        _context = context;
    }

    public int TotalMenuItems { get; set; }
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int TotalCategories { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        // Check if user is logged in
        if (HttpContext.Session.GetString("IsLoggedIn") != "true")
        {
            return RedirectToPage("/Account/Login");
        }

        // Check if user is Admin
        var userRole = HttpContext.Session.GetString("UserRole");
        if (userRole != "Admin")
        {
            return RedirectToPage("/Index");
        }

        TotalMenuItems = await _context.MenuItems.CountAsync();
        TotalOrders = await _context.Orders.CountAsync();
        PendingOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
        TotalCategories = await _context.Categories.CountAsync();

        return Page();
    }
}
