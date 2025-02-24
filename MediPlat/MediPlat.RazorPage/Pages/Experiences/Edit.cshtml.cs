using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace MediPlat.RazorPage.Pages.Experiences
{
    [Authorize(Policy = "DoctorPolicy")]
    public class EditModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly ILogger<EditModel> _logger;

        public EditModel(IHttpContextAccessor httpContextAccessor, HttpClient httpClient, ILogger<EditModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _logger = logger;
        }

        [BindProperty]
        public Experience Experience { get; set; } = default!;

        public bool IsAdmin { get; set; }
        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }
            if (token.StartsWith("Bearer "))
            {
                token = token.Substring("Bearer ".Length);
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7002/odata/Experiences/{id}?$expand=Specialty");
                if (!response.IsSuccessStatusCode)
                {
                    return Forbid();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Experience = JsonSerializer.Deserialize<Experience>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (Experience == null)
                {
                    return NotFound();
                }

                var specialtiesResponse = await _httpClient.GetAsync("https://localhost:7002/odata/Specialties");
                if (specialtiesResponse.IsSuccessStatusCode)
                {
                    var specialtiesJson = await specialtiesResponse.Content.ReadAsStringAsync();
                    var specialties = JsonSerializer.Deserialize<List<Specialty>>(specialtiesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    ViewData["SpecialtyId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(specialties, "Id", "Name", Experience.SpecialtyId);
                }

                var userRole = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
                IsAdmin = userRole == "Admin";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải thông tin kinh nghiệm.");
                return StatusCode(500, "Đã xảy ra lỗi khi tải dữ liệu.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }
            if (token.StartsWith("Bearer "))
            {
                token = token.Substring("Bearer ".Length);
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var userRole = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
                IsAdmin = userRole == "Admin";

                if (IsAdmin)
                {
                    var requestData = new { Status = Experience.Status };
                    var jsonContent = JsonSerializer.Serialize(requestData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PutAsync($"https://localhost:7002/odata/Experiences/{Experience.Id}", content);

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", $"Update failed: {errorResponse}");
                        return Page();
                    }
                }
                else
                {
                    var requestData = new
                    {
                        Title = Experience.Title,
                        Description = Experience.Description,
                        Certificate = Experience.Certificate,
                        SpecialtyId = Experience.SpecialtyId
                    };

                    var jsonContent = JsonSerializer.Serialize(requestData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PutAsync($"https://localhost:7002/odata/Experiences/{Experience.Id}", content);

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", $"Update failed: {errorResponse}");
                        return Page();
                    }
                }

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật Experience.");
                ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật.");
                return Page();
            }
        }
    }
}
