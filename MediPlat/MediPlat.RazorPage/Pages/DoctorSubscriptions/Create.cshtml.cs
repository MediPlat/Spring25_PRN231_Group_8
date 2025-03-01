using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using static MediPlat.RazorPage.Pages.Experiences.IndexModel;

namespace MediPlat.RazorPage.Pages.DoctorSubscriptions
{
    [Authorize(Policy = "DoctorPolicy")]
    public class CreateModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(IHttpContextAccessor httpContextAccessor, HttpClient httpClient, ILogger<CreateModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _logger = logger;
        }

        [BindProperty]
        public DoctorSubscriptionRequest DoctorSubscription { get; set; } = new DoctorSubscriptionRequest();
        public List<SelectListItem> SubscriptionList { get; set; } = new List<SelectListItem>();
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
                var doctorsResponse = await _httpClient.GetAsync("https://localhost:7002/odata/Doctors/profile");
                if (!doctorsResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"Lỗi khi lấy thông tin bác sĩ: {doctorsResponse.StatusCode}");
                    return Page();
                }

                var doctorJson = await doctorsResponse.Content.ReadAsStringAsync();
                var doctor = JsonSerializer.Deserialize<DoctorResponse>(doctorJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (doctor == null)
                {
                    _logger.LogError("Không thể lấy thông tin bác sĩ.");
                    return Page();
                }
                DoctorSubscription.DoctorId = doctor.Id;

                var subscriptionsResponse = await _httpClient.GetAsync("https://localhost:7002/odata/Subscriptions");
                if (!subscriptionsResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"Lỗi khi lấy danh sách Subscription: {subscriptionsResponse.StatusCode}");
                    return Page();
                }

                var subscriptionsJson = await subscriptionsResponse.Content.ReadAsStringAsync();

                var odataResponse = JsonSerializer.Deserialize<ODataResponse<SubscriptionResponse>>(subscriptionsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (odataResponse == null || odataResponse.Value == null)
                {
                    _logger.LogError("Danh sách Subscription rỗng!");
                    return Page();
                }

                var subscriptions = odataResponse.Value;
                _logger.LogInformation($"Lấy thành công {subscriptions.Count} gói Subscription.");

                ViewData["SubscriptionId"] = new SelectList(subscriptions, "Id", "Name");
                ViewData["Subscriptions"] = JsonSerializer.Serialize(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi khi tải dữ liệu: {ex.Message}");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
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
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var jsonContent = JsonSerializer.Serialize(DoctorSubscription);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://localhost:7002/odata/DoctorSubscriptions", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Không thể tạo gói đăng ký. Chi tiết lỗi: {errorResponse}");
                    return Page();
                }

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi khi tạo gói đăng ký: {ex.Message}");
                ModelState.AddModelError("", "Lỗi khi tạo gói đăng ký. Vui lòng thử lại.");
                return Page();
            }
        }
    }
}
