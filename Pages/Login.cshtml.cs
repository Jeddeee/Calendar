using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CalendarApp.Data;
using CalendarApp.Models;

namespace CalendarApp.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty] public string Username { get; set; } = "";
        [BindProperty] public string Password { get; set; } = "";
        public string? ErrorMessage { get; set; }

        public IActionResult OnPost()
        {
            var user = FakeUserStore.Users.FirstOrDefault(u => u.Username == Username && u.Password == Password);
            if (user != null)
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("Username", Username);
                return RedirectToPage("/Index");
            }

            ErrorMessage = "Invalid credentials.";
            return Page();
        }
    }
}
