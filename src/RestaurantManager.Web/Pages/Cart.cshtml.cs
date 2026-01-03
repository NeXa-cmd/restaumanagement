using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Domain.Entities;
using RestaurantManager.Infrastructure.Persistence;

namespace RestaurantManager.Web.Pages;

public class CartModel : PageModel
{
    private readonly RestaurantDbContext _dbContext;

    public CartModel(RestaurantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Cart? UserCart { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }

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

        UserCart = await _dbContext.Carts
            .Include(c => c.CartItems)
            .ThenInclude(i => i.MenuItem)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        
        if (UserCart != null && UserCart.CartItems.Any())
        {
            Subtotal = UserCart.CartItems.Sum(i => (i.MenuItem?.Price ?? 0) * i.Quantity);
            Tax = Subtotal * 0.10m;
            Total = Subtotal + Tax;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostUpdateQuantityAsync(int cartItemId, int quantity)
    {
        var cartItem = await _dbContext.CartItems.FindAsync(cartItemId);
        if (cartItem != null)
        {
            if (quantity <= 0)
            {
                _dbContext.CartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = quantity;
            }
            await _dbContext.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemoveItemAsync(int cartItemId)
    {
        var cartItem = await _dbContext.CartItems.FindAsync(cartItemId);
        if (cartItem != null)
        {
            _dbContext.CartItems.Remove(cartItem);
            await _dbContext.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCheckoutAsync()
    {
        var userIdStr = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            return RedirectToPage("/Account/Login");
        }

        var cart = await _dbContext.Carts
            .Include(c => c.CartItems)
            .ThenInclude(i => i.MenuItem)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null || !cart.CartItems.Any())
        {
            return RedirectToPage();
        }

        // Generate a unique order number
        var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{userId}";

        var order = new Order
        {
            OrderNumber = orderNumber,
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Preparing,
            TotalAmount = cart.CartItems.Sum(i => (i.MenuItem?.Price ?? 0) * i.Quantity) * 1.10m,
            OrderItems = cart.CartItems.Select(i => new OrderItem
            {
                MenuItemId = i.MenuItemId,
                Quantity = i.Quantity,
                UnitPrice = i.MenuItem?.Price ?? 0,
                TotalPrice = (i.MenuItem?.Price ?? 0) * i.Quantity
            }).ToList()
        };

        _dbContext.Orders.Add(order);
        _dbContext.CartItems.RemoveRange(cart.CartItems);
        await _dbContext.SaveChangesAsync();

        return RedirectToPage("/Orders", new { filter = "active" });
    }
}
