using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Infrastructure.Persistence;

namespace RestaurantManager.Web.Pages.Account;

public class LoginModel : PageModel
{
    private readonly RestaurantDbContext _dbContext;

    public LoginModel(RestaurantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [BindProperty]
    public string Email { get; set; } = string.Empty;
    
    [BindProperty]
    public string Password { get; set; } = string.Empty;
    
    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        // If already logged in, redirect to dashboard
        if (HttpContext.Session.GetString("IsLoggedIn") == "true")
        {
            return RedirectToPage("/Index");
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Check credentials against database
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == Email && u.PasswordHash == Password);
        
        if (user != null)
        {
            // Set session
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("IsLoggedIn", "true");
            
            return RedirectToPage("/Index");
        }
        
        ErrorMessage = "Invalid email or password. Please try again.";
        return Page();
    }
}
