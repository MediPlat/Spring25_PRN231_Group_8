using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using MediPlat.Model.ResponseObject;
using Microsoft.AspNetCore.Authorization;
using static MediPlat.RazorPage.Pages.Experiences.IndexModel;

namespace MediPlat.RazorPage.Pages.DoctorSubscriptions
{
    [Authorize(Policy = "DoctorPolicy")]
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

        public IList<DoctorSubscriptionResponse> DoctorSubscriptions { get; set; } = new List<DoctorSubscriptionResponse>();
        public string DoctorFullName { get; set; } = "Chưa có thông tin bác sĩ";

        public async Task<IActionResult> OnGetAsync()
        {
            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var doctorResponse = await _httpClient.GetAsync("https://localhost:7002/odata/Doctors/profile");

                if (doctorResponse.IsSuccessStatusCode)
                {
                    var doctorJson = await doctorResponse.Content.ReadAsStringAsync();
                    var doctor = JsonSerializer.Deserialize<DoctorResponse>(doctorJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (doctor != null)
                    {
                        DoctorFullName = doctor.FullName;
                        _logger.LogInformation($"Lấy thành công thông tin bác sĩ: {DoctorFullName}");
                    }
                }
                else
                {
                    _logger.LogWarning("Không thể lấy thông tin bác sĩ.");
                }

                var subscriptionsResponse = await _httpClient.GetAsync("https://localhost:7002/odata/DoctorSubscriptions?$expand=Doctor,Subscription");

                if (subscriptionsResponse.IsSuccessStatusCode)
                {
                    var jsonResponse = await subscriptionsResponse.Content.ReadAsStringAsync();
                    var odataResponse = JsonSerializer.Deserialize<ODataResponse<DoctorSubscriptionResponse>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    DoctorSubscriptions = odataResponse?.Value ?? new List<DoctorSubscriptionResponse>();
                    _logger.LogInformation($"Lấy thành công {DoctorSubscriptions.Count} gói đăng ký.");
                }
                else
                {
                    _logger.LogError($"Lỗi khi lấy danh sách gói đăng ký: {subscriptionsResponse.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tải dữ liệu gói đăng ký: {ex.Message}");
                return StatusCode(500, "Lỗi máy chủ khi tải danh sách gói đăng ký.");
            }

            ViewData["Title"] = "Danh sách gói đăng ký của bác sĩ";
            return Page();
        }
    }
}
