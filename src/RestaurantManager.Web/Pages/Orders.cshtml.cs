using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Domain.Entities;
using RestaurantManager.Infrastructure.Persistence;

namespace RestaurantManager.Web.Pages;

public class OrdersModel : PageModel
{
    private readonly RestaurantDbContext _dbContext;

    public OrdersModel(RestaurantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<Order> DisplayedOrders { get; set; } = new();
    public string CurrentFilter { get; set; } = "active";
    public int ActiveOrdersCount { get; set; }
    public int PastOrdersCount { get; set; }

    public async Task<IActionResult> OnGetAsync(string filter = "active")
    {
        if (HttpContext.Session.GetString("IsLoggedIn") != "true")
        {
            return RedirectToPage("/Account/Login");
        }

        var userIdStr = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            return RedirectToPage("/Account/Login");
        }

        CurrentFilter = filter;
        
        var userOrders = _dbContext.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.MenuItem)
            .Where(o => o.UserId == userId);
        
        ActiveOrdersCount = await userOrders.CountAsync(o => o.Status == OrderStatus.Preparing || o.Status == OrderStatus.Ready);
        PastOrdersCount = await userOrders.CountAsync(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Cancelled);
        
        if (filter == "active")
        {
            DisplayedOrders = await userOrders
                .Where(o => o.Status == OrderStatus.Preparing || o.Status == OrderStatus.Ready)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
        else
        {
            DisplayedOrders = await userOrders
                .Where(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Cancelled)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        return Page();
    }
}
