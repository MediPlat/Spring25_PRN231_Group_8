using System.Net.Http.Headers;
using System.Text.Json;
using MediPlat.Model.ResponseObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public ExperienceResponse? Experience { get; set; }

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
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                string apiUrl = $"https://localhost:7002/odata/Experiences/{id}?$expand=Doctor,Specialty";
                _logger.LogInformation($"Fetching Experience details from: {apiUrl}");

                var response = await _httpClient.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"API Error: {response.StatusCode} - {errorResponse}");

                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return NotFound("Experience không tồn tại.");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return RedirectToPage("/Auth/Login");
                    }
                    return StatusCode((int)response.StatusCode, "Lỗi khi tải chi tiết Experience.");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Experience = JsonSerializer.Deserialize<ExperienceResponse>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (Experience == null)
                {
                    _logger.LogWarning($"Experience với ID {id} không có dữ liệu hợp lệ.");
                    return NotFound("Experience không có dữ liệu hợp lệ.");
                }

                _logger.LogInformation($"Loaded Experience: ID={Experience.Id}, Title={Experience.Title}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tải Experience: {ex.Message}");
                return StatusCode(500, "Lỗi máy chủ khi tải chi tiết Experience.");
            }

            return Page();
        }
    }
}
