using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestaurantManager.Domain.Entities;
using RestaurantManager.Infrastructure.Persistence;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManager.Web.Pages.Admin.Categories;

public class CreateModel : PageModel
{
    private readonly RestaurantDbContext _context;

    public CreateModel(RestaurantDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Display order is required")]
        [Range(1, 100, ErrorMessage = "Display order must be between 1 and 100")]
        public int DisplayOrder { get; set; } = 1;

        public bool IsActive { get; set; } = true;
    }

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetString("IsLoggedIn") != "true")
        {
            return RedirectToPage("/Account/Login");
        }

        if (HttpContext.Session.GetString("UserRole") != "Admin")
        {
            return RedirectToPage("/Index");
        }

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
            return Page();
        }

        var category = new Category
        {
            Name = Input.Name,
            Description = Input.Description,
            DisplayOrder = Input.DisplayOrder,
            IsActive = Input.IsActive
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
