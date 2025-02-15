using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace MediPlat.RazorPage.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly HttpClient _httpClient = new HttpClient();

        [BindProperty]
        public RegisterRequest RegisterInput { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(RegisterInput),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("https://localhost:7002/api/auth/register", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Redirect("~/"); // Redirects to the Index page dynamically
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            ErrorMessage = !string.IsNullOrEmpty(errorResponse) ? errorResponse : "Registration failed.";
            return Page();
        }

        public class RegisterRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}
