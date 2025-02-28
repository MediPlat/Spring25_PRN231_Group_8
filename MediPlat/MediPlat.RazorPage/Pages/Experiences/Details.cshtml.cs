using System.Net.Http.Headers;
using System.Text.Json;
using MediPlat.Model.Model;
using Microsoft.AspNetCore.Authorization;
using MediPlat.Model.ResponseObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static MediPlat.RazorPage.Pages.Experiences.IndexModel;

namespace MediPlat.RazorPage.Pages.Experiences
{
    [Authorize(Policy = "DoctorOrAdminorPatientPolicy")]
    public class DetailsModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(IHttpContextAccessor httpContextAccessor, HttpClient httpClient, ILogger<DetailsModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _logger = logger;
        }
        public class ODataResponse<T>
        {
            public string? Context { get; set; }
            public List<T> Value { get; set; } = new List<T>();
        }
        public ExperienceResponse Experience { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound("Experience ID is required.");
            }

            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7002/odata/Experiences/{id}?$expand=Doctor,Specialty");

                if (!response.IsSuccessStatusCode)
                {
                    return Forbid();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                // Deserialize thành một đối tượng tạm thời để lấy mảng "value"
                var odataResponse = JsonSerializer.Deserialize<ODataResponse<ExperienceResponse>>(jsonResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Lấy đối tượng đầu tiên từ mảng "value"
                Experience = odataResponse?.Value?.FirstOrDefault() ?? new ExperienceResponse();

                // Không cần gán giá trị mặc định nữa, vì dữ liệu đã được deserialize đúng
                // Experience.Doctor ??= new DoctorResponse { FullName = "Không có thông tin" };
                // Experience.Specialty ??= new SpecialtyResponse { Name = "Không có chuyên khoa" };

                _logger.LogInformation($"Experience: {JsonSerializer.Serialize(Experience, new JsonSerializerOptions { WriteIndented = true })}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tải dữ liệu Experience: {ex.Message}");
                return StatusCode(500, "Lỗi máy chủ khi lấy dữ liệu Experience.");
            }

            return Page();
        }
    }
}
