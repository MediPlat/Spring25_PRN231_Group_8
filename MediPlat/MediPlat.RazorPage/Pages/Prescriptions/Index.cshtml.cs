using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static MediPlat.RazorPage.Pages.Experiences.IndexModel;

namespace MediPlat.RazorPage.Pages.Prescriptions
{
    [Authorize(Policy = "DoctorOrAdminOrPatientPolicy")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor, ILogger<IndexModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public List<AppointmentSlotResponse> Prescriptions { get; set; } = new List<AppointmentSlotResponse>();

        public async Task<IActionResult> OnGetAsync()
        {
            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }

            var client = _clientFactory.CreateClient("UntrustedClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string apiUrl = "https://localhost:7002/odata/AppointmentSlots?$expand=AppointmentSlotMedicines($expand=Medicine),Profile";

            if (User.IsInRole("Doctor"))
            {
                var doctorId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                apiUrl = $"https://localhost:7002/odata/AppointmentSlots/doctor/{doctorId}";
            }
            else if (User.IsInRole("Patient"))
            {
                var profileId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                apiUrl = $"https://localhost:7002/odata/AppointmentSlots/patient/{profileId}";
            }

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"API Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    return StatusCode((int)response.StatusCode, "Lỗi khi tải danh sách đơn thuốc.");
                }

                var apiResponse = await response.Content.ReadAsStringAsync();
                var prescriptionData = JsonSerializer.Deserialize<ODataResponse<AppointmentSlotResponse>>(apiResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                Prescriptions = prescriptionData?.Value ?? new List<AppointmentSlotResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tải dữ liệu đơn thuốc: {ex.Message}");
                return StatusCode(500, "Lỗi máy chủ khi tải danh sách đơn thuốc.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid appointmentSlotId, string status)
        {
            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }

            var client = _clientFactory.CreateClient("UntrustedClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var doctorId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            var getResponse = await client.GetAsync($"https://localhost:7002/odata/AppointmentSlots/doctor/{doctorId}/slot/{appointmentSlotId}");
            if (!getResponse.IsSuccessStatusCode)
            {
                _logger.LogError($"⚠ Không tìm thấy đơn thuốc với ID: {appointmentSlotId}");
                return Page();
            }

            var apiResponse = await getResponse.Content.ReadAsStringAsync();
            var existingPrescription = JsonSerializer.Deserialize<AppointmentSlotResponse>(apiResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (existingPrescription == null)
            {
                _logger.LogError($"⚠ Không tìm thấy đơn thuốc với ID: {appointmentSlotId}");
                return Page();
            }

            if (existingPrescription.Status == "Confirmed")
            {
                _logger.LogWarning($"⛔ Không thể cập nhật trạng thái vì đơn thuốc đã được xác nhận.");
                return Page();
            }

            var updateData = new { Status = status };
            var jsonContent = new StringContent(JsonSerializer.Serialize(updateData), Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"https://localhost:7002/odata/AppointmentSlots/{appointmentSlotId}/status", jsonContent);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"⚠ Lỗi cập nhật trạng thái: {await response.Content.ReadAsStringAsync()}");
            }

            await OnGetAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("❌ Không tìm thấy đơn thuốc vì ID rỗng.");
                return Page();
            }

            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }

            var client = _clientFactory.CreateClient("UntrustedClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"https://localhost:7002/odata/AppointmentSlots/{id}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"❌ Lỗi khi xóa đơn thuốc ID {id}: {await response.Content.ReadAsStringAsync()}");
                return Page();
            }

            return RedirectToPage();
        }
    }
}