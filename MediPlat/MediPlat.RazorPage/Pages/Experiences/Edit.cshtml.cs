using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MediPlat.Model.RequestObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using MediPlat.Model.ResponseObject;
using static MediPlat.RazorPage.Pages.Experiences.DetailsModel;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public ExperienceRequest Experience { get; set; } = default!;
        public bool IsAdmin { get; set; }
        public string DoctorFullName { get; set; } = string.Empty;
        public string SpecialtyName { get; set; } = string.Empty;
        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7002/odata/Experiences/{id}?$expand=Specialty,Doctor");

                if (!response.IsSuccessStatusCode)
                {
                    return Forbid();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var odataResponse = JsonSerializer.Deserialize<ODataResponse<ExperienceResponse>>(jsonResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var experienceResponse = odataResponse?.Value?.FirstOrDefault();

                if (experienceResponse == null)
                {
                    return NotFound();
                }

                Experience = new ExperienceRequest
                {
                    SpecialtyId = experienceResponse.SpecialtyId,
                    Title = experienceResponse.Title,
                    Description = experienceResponse.Description,
                    Certificate = experienceResponse.Certificate,
                    Status = experienceResponse.Status,
                    DoctorId = experienceResponse.DoctorId
                };

                DoctorFullName = experienceResponse.Doctor?.FullName ?? "Không có thông tin";
                SpecialtyName = experienceResponse.Specialty?.Name ?? "Không có chuyên khoa";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải thông tin kinh nghiệm.");
                return StatusCode(500, "Đã xảy ra lỗi khi tải dữ liệu.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
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

                    var response = await _httpClient.PutAsync($"https://localhost:7002/odata/Experiences/{id}", content);

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
                        SpecialtyId = Experience.SpecialtyId,
                        DoctorId = Experience.DoctorId
                    };

                    var jsonContent = JsonSerializer.Serialize(requestData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    _logger.LogInformation($"DoctorId before sending API: {Experience.DoctorId}");
                    var response = await _httpClient.PutAsync($"https://localhost:7002/odata/Experiences/{id}", content);

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
