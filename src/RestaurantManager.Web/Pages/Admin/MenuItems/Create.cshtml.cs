using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Domain.Entities;
using RestaurantManager.Infrastructure.Persistence;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManager.Web.Pages.Admin.MenuItems;

public class CreateModel : PageModel
{
    private readonly RestaurantDbContext _context;

    public CreateModel(RestaurantDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<string> Categories { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10000")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; } = string.Empty;

        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? ImageUrl { get; set; }

        public bool IsAvailable { get; set; } = true;
    }

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

        Categories = await _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .Select(c => c.Name)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (HttpContext.Session.GetString("IsLoggedIn") != "true")
        {
            return RedirectToPage("/Account/Login");
        }

        if (HttpContext.Session.GetString("UserRole") != "Admin")
        {
            return RedirectToPage("/Index");
        }

        if (!ModelState.IsValid)
        {
            Categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .Select(c => c.Name)
                .ToListAsync();
            return Page();
        }

        var menuItem = new MenuItem
        {
            Name = Input.Name,
            Description = Input.Description,
            Price = Input.Price,
            Category = Input.Category,
            ImageUrl = Input.ImageUrl ?? string.Empty,
            IsAvailable = Input.IsAvailable,
            CreatedAt = DateTime.UtcNow
        };

        _context.MenuItems.Add(menuItem);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
