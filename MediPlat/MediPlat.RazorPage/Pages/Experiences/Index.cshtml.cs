using System.Net.Http.Headers;
using System.Text.Json;
using MediPlat.Model.ResponseObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MediPlat.RazorPage.Pages.Experiences
{
    [Authorize(Policy = "DoctorOrAdminorPatientPolicy")]
    public class IndexModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IHttpContextAccessor httpContextAccessor, HttpClient httpClient, ILogger<IndexModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _logger = logger;
        }

        public List<ExperienceResponse> Experiences { get; set; } = new List<ExperienceResponse>();
        public string DoctorFullName { get; set; } = "Chưa có thông tin bác sĩ";

        public int PageSize { get; set; } = 5;
        public int CurrentPage { get; set; } = 1;
        public int TotalItems { get; set; }

        public class ODataResponse<T>
        {
            public List<T>? Value { get; set; }
            public int? Count { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int page = 1)
        {
            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                string apiUrl = $"https://localhost:7002/odata/Experiences?$count=true&$top={PageSize}&$skip={(page - 1) * PageSize}&$expand=Doctor,Specialty";
                _logger.LogInformation($"Fetching data from: {apiUrl}");

                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"API Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    return StatusCode((int)response.StatusCode, "Lỗi khi tải danh sách Experience.");
                }

                var apiResponse = await response.Content.ReadAsStringAsync();
                var experienceData = JsonSerializer.Deserialize<ODataResponse<ExperienceResponse>>(apiResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                Experiences = experienceData?.Value ?? new List<ExperienceResponse>();
                TotalItems = experienceData?.Count ?? Experiences.Count;

                _logger.LogInformation($"Loaded {Experiences.Count} Experiences, TotalItems: {TotalItems}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tải dữ liệu Experience: {ex.Message}");
                return StatusCode(500, "Lỗi máy chủ khi tải danh sách Experience.");
            }

            CurrentPage = page;
            ViewData["Title"] = $"Danh sách Experience của {DoctorFullName}";
            return Page();
        }
    }
}
