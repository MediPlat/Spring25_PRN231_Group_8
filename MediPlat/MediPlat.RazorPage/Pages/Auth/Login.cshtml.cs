using MediPlat.Model.Authen_Athor;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

public class LoginModel : PageModel
{
    private readonly HttpClient _httpClient;

    public LoginModel()
    {
        _httpClient = new HttpClient();
    }

    [BindProperty]
    public LoginRequest LoginRequestModel { get; set; } = new LoginRequest();

    public string ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var content = new StringContent(JsonConvert.SerializeObject(LoginRequestModel), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://localhost:7002/api/auth/login", content);

        if (response.IsSuccessStatusCode)
        {
            var apiResponse = await response.Content.ReadAsStringAsync();
            var authResult = JsonConvert.DeserializeObject<AuthResult>(apiResponse);

            if (!string.IsNullOrEmpty(authResult.Token))
            {
                Response.Cookies.Append("AuthToken", authResult.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = authResult.ExpiresAt
                });

                var token = authResult.Token;
                if (token.StartsWith("Bearer "))
                {
                    token = token.Substring("Bearer ".Length);
                }

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                var userRole = roleClaim?.Value;

                Console.WriteLine($"User logged in with role: {userRole}");

                return userRole switch
                {
                    "Doctor" => RedirectToPage("/DoctorSubscriptions/Index"),
                    "Admin" => RedirectToPage("/Medicines/Index"),
                    "Patient" => RedirectToPage("/PatientPages/Index"),
                    _ => RedirectToPage("/Index")
                };
            }
        }

        ErrorMessage = "Invalid login credentials";
        return Page();
    }

}

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}
