using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestaurantManager.Domain.Entities;
using RestaurantManager.Infrastructure.Persistence;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManager.Web.Pages.Admin.Categories;

public class EditModel : PageModel
{
    private readonly RestaurantDbContext _context;

    public EditModel(RestaurantDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Display order is required")]
        [Range(1, 100, ErrorMessage = "Display order must be between 1 and 100")]
        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (HttpContext.Session.GetString("IsLoggedIn") != "true")
        {
            return RedirectToPage("/Account/Login");
        }

        if (HttpContext.Session.GetString("UserRole") != "Admin")
        {
            return RedirectToPage("/Index");
        }

        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive
        };

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

        var category = await _context.Categories.FindAsync(Input.Id);
        if (category == null)
        {
            return NotFound();
        }

        category.Name = Input.Name;
        category.Description = Input.Description;
        category.DisplayOrder = Input.DisplayOrder;
        category.IsActive = Input.IsActive;

        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
