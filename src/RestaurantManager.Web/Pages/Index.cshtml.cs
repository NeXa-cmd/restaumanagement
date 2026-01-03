using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Domain.Entities;
using RestaurantManager.Infrastructure.Persistence;

namespace RestaurantManager.Web.Pages;

public class IndexModel : PageModel
{
    private readonly RestaurantDbContext _dbContext;

    public IndexModel(RestaurantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<MenuItem> FilteredMenuItems { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    
    [BindProperty(SupportsGet = true)]
    public List<string> SelectedCategories { get; set; } = new();

    public async Task<IActionResult> OnGetAsync([FromQuery] List<string> categories)
    {
        // Check if user is logged in
        if (HttpContext.Session.GetString("IsLoggedIn") != "true")
        {
            return RedirectToPage("/Account/Login");
        }

        Categories = await _dbContext.MenuItems.Select(m => m.Category).Distinct().OrderBy(c => c).ToListAsync();
        SelectedCategories = categories ?? new List<string>();
        
        var query = _dbContext.MenuItems.AsQueryable();
        
        if (SelectedCategories.Any())
        {
            query = query.Where(m => SelectedCategories.Contains(m.Category));
        }
        
        FilteredMenuItems = await query.OrderBy(m => m.Name).ToListAsync();

        return Page();
    }

    public int GetCategoryCount(string category)
    {
        return _dbContext.MenuItems.Count(m => m.Category == category);
    }
    
    public async Task<IActionResult> OnPostAddToCartAsync(int menuItemId)
    {
        var userIdStr = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            return RedirectToPage("/Account/Login");
        }

        var cart = await _dbContext.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart { UserId = userId, CartItems = new List<CartItem>() };
            _dbContext.Carts.Add(cart);
        }

        var existingItem = cart.CartItems.FirstOrDefault(i => i.MenuItemId == menuItemId);
        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            cart.CartItems.Add(new CartItem { MenuItemId = menuItemId, Quantity = 1 });
        }

        await _dbContext.SaveChangesAsync();
        return RedirectToPage();
    }
}
