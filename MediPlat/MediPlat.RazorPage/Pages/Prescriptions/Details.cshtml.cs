using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MediPlat.RazorPage.Pages.Prescriptions
{
    [Authorize(Policy = "DoctorOrAdminOrPatientPolicy")]
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor, ILogger<DetailsModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public AppointmentSlotResponse Prescription { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }
            var client = _clientFactory.CreateClient("UntrustedClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string apiUrl = "";

            if (User.IsInRole("Doctor"))
            {
                var doctorId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                apiUrl = $"https://localhost:7002/odata/AppointmentSlots/doctor/{doctorId}/slot/{id}?$expand=AppointmentSlotMedicine($expand=Medicine)";
            }
            else if (User.IsInRole("Patient"))
            {
                var profileId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation($"📌 ProfileId: {profileId}");
                apiUrl = $"https://localhost:7002/odata/AppointmentSlots/patient/{profileId}/slot/{id}";
            }
            else
            {
                _logger.LogError("❌ User không có quyền xem đơn thuốc.");
                return Forbid();
            }

            var response = await client.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"⚠ API Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                return NotFound();
            }

            var apiResponse = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"📜 API Response: {apiResponse}");
            Prescription = JsonSerializer.Deserialize<AppointmentSlotResponse>(apiResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            _logger.LogInformation($"🟢 Prescription has {Prescription.AppointmentSlotMedicines?.Count} medicines.");
            return Page();
        }
    }
}