using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Domain.Entities;
using RestaurantManager.Infrastructure.Persistence;

namespace RestaurantManager.Web.Pages;

public class ProfileModel : PageModel
{
    private readonly RestaurantDbContext _dbContext;

    public ProfileModel(RestaurantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public User? CurrentUser { get; set; }
    public int ActiveOrdersCount { get; set; }
    public int CompletedOrdersCount { get; set; }
    public decimal TotalSpent { get; set; }

    public async Task<IActionResult> OnGetAsync()
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

        CurrentUser = await _dbContext.Users.FindAsync(userId);
        
        var userOrders = _dbContext.Orders.Where(o => o.UserId == userId);
        ActiveOrdersCount = await userOrders.CountAsync(o => o.Status == OrderStatus.Preparing || o.Status == OrderStatus.Ready);
        CompletedOrdersCount = await userOrders.CountAsync(o => o.Status == OrderStatus.Completed);
        TotalSpent = await userOrders.Where(o => o.Status == OrderStatus.Completed).SumAsync(o => o.TotalAmount);

        return Page();
    }
}
