using MediPlat.Model.Authen_Athor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

public class LoginModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LoginModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
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

        var client = _httpClientFactory.CreateClient("UntrustedClient");
        var content = new StringContent(JsonConvert.SerializeObject(LoginRequestModel), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("api/auth/login", content);

        if (response.IsSuccessStatusCode)
        {
            var apiResponse = await response.Content.ReadAsStringAsync();
            var authResult = JsonConvert.DeserializeObject<AuthResult>(apiResponse);

            if (!string.IsNullOrEmpty(authResult?.Token))
            {
                Response.Cookies.Append("AuthToken", authResult.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = authResult.ExpiresAt
                });

                var token = authResult.Token.StartsWith("Bearer ") ? authResult.Token.Substring("Bearer ".Length) : authResult.Token;

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                var userRole = roleClaim?.Value;

                Console.WriteLine($"User logged in with role: {userRole}");

                return userRole switch
                {

                    "Doctor" => RedirectToPage("/Doctors/Profile"),
                    "Admin" => RedirectToPage("/Admin/Index"),
                    "Patient" => RedirectToPage("/Prescriptions/Index"),
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
    public string? Email { get; set; }
    public string? Password { get; set; }
}
