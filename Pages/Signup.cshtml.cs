using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CalendarApp.Data;
using CalendarApp.Models;

namespace CalendarApp.Pages
{
    public class SignupModel : PageModel
    {
        [BindProperty] public string Username { get; set; } = "";
        [BindProperty] public string Password { get; set; } = "";
        public string? Message { get; set; }

        public IActionResult OnPost()
        {
            if (FakeUserStore.Users.Any(u => u.Username == Username))
            {
                Message = "Username already taken.";
                return Page();
            }

            FakeUserStore.Users.Add(new User { Username = Username, Password = Password });
            return RedirectToPage("/Login");
        }
    }
}
