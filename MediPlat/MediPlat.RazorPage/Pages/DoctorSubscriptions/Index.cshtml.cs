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
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<IndexModel> _logger;
        public class ODataSingleResponse<T>
        {
            public T? Value { get; set; }
        }

        public IndexModel(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory, ILogger<IndexModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public List<DoctorSubscriptionResponse> DoctorSubscriptions { get; set; } = new List<DoctorSubscriptionResponse>();
        public string DoctorFullName { get; set; } = "Chưa có thông tin bác sĩ";

        public async Task<IActionResult> OnGetAsync()
        {
            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }

            var client = _clientFactory.CreateClient("UntrustedClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var doctorResponse = await client.GetAsync("https://localhost:7002/odata/Doctors/profile");

                if (doctorResponse.IsSuccessStatusCode)
                {
                    var doctorJson = await doctorResponse.Content.ReadAsStringAsync();
                    var doctor = JsonSerializer.Deserialize<DoctorResponse>(doctorJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (doctor != null)
                    {
                        DoctorFullName = doctor.FullName;
                    }
                }
                else
                {
                    _logger.LogWarning("Không thể lấy thông tin bác sĩ.");
                }

                var subscriptionsResponse = await client.GetAsync("https://localhost:7002/odata/DoctorSubscriptions?$expand=Doctor,Subscription");

                if (subscriptionsResponse.IsSuccessStatusCode)
                {
                    var jsonResponse = await subscriptionsResponse.Content.ReadAsStringAsync();
                    var odataResponse = JsonSerializer.Deserialize<ODataResponse<DoctorSubscriptionResponse>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    DoctorSubscriptions = odataResponse?.Value ?? new List<DoctorSubscriptionResponse>();
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
