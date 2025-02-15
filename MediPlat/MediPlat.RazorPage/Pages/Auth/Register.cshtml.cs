using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;

namespace MediPlat.RazorPage.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public RegisterModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty] public string? UserName { get; set; }
        [BindProperty] public string? FullName { get; set; }

        [BindProperty]
        [Required, EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required, RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$",
            ErrorMessage = "Password must have at least 8 characters, 1 uppercase, 1 lowercase, 1 digit, and 1 special character.")]
        public string Password { get; set; } = string.Empty;

        [BindProperty] public string? PhoneNumber { get; set; }
        [BindProperty] public string? Sex { get; set; }
        [BindProperty] public string? Address { get; set; }

        public string? EmailError { get; private set; }
        public string? PasswordError { get; private set; }
        public string? ServerError { get; private set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState)
                {
                    if (modelState.Key == nameof(Email) && modelState.Value.Errors.Count > 0)
                        EmailError = modelState.Value.Errors[0].ErrorMessage;

                    if (modelState.Key == nameof(Password) && modelState.Value.Errors.Count > 0)
                        PasswordError = modelState.Value.Errors[0].ErrorMessage;
                }

                return Page();
            }

            var registerRequest = new
            {
                UserName,
                FullName,
                Email,
                Password,
                PhoneNumber,
                Sex,
                Address
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(registerRequest), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("https://localhost:7002/api/auth/register", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/Index");
                }

                var errorResponse = await response.Content.ReadAsStringAsync();
                ServerError = ExtractErrorMessage(errorResponse);
            }
            catch
            {
                ServerError = "An error occurred. Please try again.";
            }

            return Page();
        }

        private string ExtractErrorMessage(string jsonResponse)
        {
            try
            {
                var doc = JsonDocument.Parse(jsonResponse);
                if (doc.RootElement.TryGetProperty("errors", out var errors))
                {
                    foreach (var error in errors.EnumerateObject())
                    {
                        return error.Value[0].GetString() ?? "Registration failed.";
                    }
                }
                return "Registration failed.";
            }
            catch
            {
                return "Invalid response from server.";
            }
        }
    }
}
