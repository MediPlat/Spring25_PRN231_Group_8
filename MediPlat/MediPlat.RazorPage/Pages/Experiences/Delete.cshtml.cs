using System.Net.Http.Headers;
using System.Text.Json;
using MediPlat.Model.ResponseObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static MediPlat.RazorPage.Pages.Experiences.DetailsModel;

namespace MediPlat.RazorPage.Pages.Experiences
{
    [Authorize(Policy = "DoctorPolicy")]
    public class DeleteModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(IHttpContextAccessor httpContextAccessor, HttpClient httpClient, ILogger<DeleteModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _logger = logger;
        }

        [BindProperty]
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
                _logger.LogInformation($"Fetching Experience with ID: {id}");
                var response = await _httpClient.GetAsync($"https://localhost:7002/odata/Experiences/{id}?$expand=Specialty,Doctor");
                _logger.LogInformation($"API Request URL: https://localhost:7002/odata/Experiences/{id}?$expand=Doctor,Specialty");
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound("Experience không tồn tại.");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"API Response: {jsonResponse}");
                var experienceResponse = JsonSerializer.Deserialize<ODataResponse<ExperienceResponse>>
                    (jsonResponse,new JsonSerializerOptions { PropertyNameCaseInsensitive = true })?.Value?.FirstOrDefault();
                _logger.LogInformation($"Experience sau khi Deserialize: {JsonSerializer.Serialize(experienceResponse)}");
                if (experienceResponse == null)
                {
                    return NotFound("Experience không tồn tại.");
                }

                Experience = experienceResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tải dữ liệu Experience: {ex.Message}");
                return StatusCode(500, "Lỗi máy chủ khi lấy dữ liệu Experience.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
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
                var response = await _httpClient.DeleteAsync($"https://localhost:7002/odata/Experiences/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Không thể xóa Experience. Chi tiết lỗi: {errorResponse}");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi xóa Experience: {ex.Message}");
                ModelState.AddModelError("", "Lỗi khi xóa Experience. Vui lòng thử lại.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
