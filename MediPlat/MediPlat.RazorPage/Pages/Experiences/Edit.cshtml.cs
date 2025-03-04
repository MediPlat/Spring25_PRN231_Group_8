using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MediPlat.Model.RequestObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using MediPlat.Model.ResponseObject;

namespace MediPlat.RazorPage.Pages.Experiences
{
    [Authorize(Policy = "DoctorOrAdminPolicy")]
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<EditModel> _logger;

        public EditModel(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory, ILogger<EditModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        [BindProperty]
        public ExperienceRequest Experience { get; set; } = new ExperienceRequest();
        public bool IsAdmin { get; private set; } = false;
        public bool IsDoctor { get; private set; } = false;
        public string DoctorFullName { get; set; } = string.Empty;
        public string SpecialtyName { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound("Experience ID không hợp lệ.");
            }

            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }
            var client = _clientFactory.CreateClient("UntrustedClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var userRole = User.FindFirstValue(ClaimTypes.Role);
            IsAdmin = userRole == "Admin";
            IsDoctor = userRole == "Doctor";
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            try
            {
                var response = await client.GetAsync($"https://localhost:7002/odata/Experiences/{id}?$expand=Specialty,Doctor");

                if (!response.IsSuccessStatusCode)
                {
                    return Forbid();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var experienceResponse = JsonSerializer.Deserialize<ExperienceResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (experienceResponse == null)
                {
                    return NotFound();
                }

                // Nếu là Doctor nhưng không phải chủ sở hữu của Experience → Không có quyền chỉnh sửa
                if (IsDoctor && experienceResponse.DoctorId != userId)
                {
                    return Forbid();
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

            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }
            var client = _clientFactory.CreateClient("UntrustedClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var userRole = User.FindFirstValue(ClaimTypes.Role);
            IsAdmin = userRole == "Admin";
            IsDoctor = userRole == "Doctor";

            try
            {
                var requestData = new Dictionary<string, object>();

                if (IsAdmin)
                {
                    // Admin chỉ có thể sửa Status
                    requestData["Status"] = Experience.Status;
                }
                else if (IsDoctor)
                {
                    // Doctor chỉ có thể sửa nội dung, không được chỉnh sửa Status
                    requestData["Title"] = Experience.Title;
                    requestData["Description"] = Experience.Description;
                    requestData["Certificate"] = Experience.Certificate;
                    requestData["SpecialtyId"] = Experience.SpecialtyId;
                    requestData["DoctorId"] = Experience.DoctorId;
                }
                else
                {
                    return Forbid();
                }

                var jsonContent = JsonSerializer.Serialize(requestData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"https://localhost:7002/odata/Experiences/{id}", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Update failed: {errorResponse}");
                    return Page();
                }

                _logger.LogInformation($"Experience '{Experience.Title}' đã được cập nhật thành công.");
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
