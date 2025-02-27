using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MediPlat.Model.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        public async Task<IActionResult> OnGetAsync()
        {
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
                var doctorsResponse = await _httpClient.GetAsync("https://localhost:7002/odata/Doctors/profile");
                var subscriptionsResponse = await _httpClient.GetAsync("https://localhost:7002/odata/Subscriptions");

                if (!doctorsResponse.IsSuccessStatusCode || !subscriptionsResponse.IsSuccessStatusCode)
                {
                    return Page();
                }

                var doctorsJson = await doctorsResponse.Content.ReadAsStringAsync();
                var subscriptionsJson = await subscriptionsResponse.Content.ReadAsStringAsync();

                var doctor = JsonSerializer.Deserialize<Doctor>(doctorsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (doctor != null)
                {
                    ViewData["DoctorId"] = doctor.Id.ToString();
                }

                if (DoctorSubscription == null)
                {
                    DoctorSubscription = new DoctorSubscription();
                }

                if (ViewData["DoctorId"] != null && Guid.TryParse(ViewData["DoctorId"].ToString(), out Guid parsedDoctorId))
                {
                    DoctorSubscription.DoctorId = parsedDoctorId;
                }

                var subscriptions = JsonSerializer.Deserialize<List<Subscription>>(subscriptionsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                ViewData["SubscriptionId"] = new SelectList(subscriptions, "Id", "Name");
                ViewData["Subscriptions"] = subscriptions;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi khi tải dữ liệu: {ex.Message}");
            }

            return Page();
        }

        [BindProperty]
        public DoctorSubscription DoctorSubscription { get; set; } = default!;

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
                ModelState.AddModelError("", "Lỗi khi tạo gói đăng ký. Vui lòng thử lại.");
                return Page();
            }
        }
    }
}