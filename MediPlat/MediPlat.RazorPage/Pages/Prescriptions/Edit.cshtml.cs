using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using static MediPlat.RazorPage.Pages.Experiences.IndexModel;

namespace MediPlat.RazorPage.Pages.Prescriptions
{
    [Authorize(Policy = "DoctorPolicy")]
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<EditModel> _logger;

        public EditModel(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor, ILogger<EditModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        [BindProperty]
        public AppointmentSlotRequest AppointmentSlot { get; set; } = new AppointmentSlotRequest();

        public List<MedicineResponse> Medicines { get; set; } = new List<MedicineResponse>();

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }

            var client = _clientFactory.CreateClient("UntrustedClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var doctorId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (doctorId == null)
            {
                return Unauthorized();
            }

            // Gọi API để lấy thông tin đơn thuốc
            var response = await client.GetAsync($"https://localhost:7002/odata/AppointmentSlots/doctor/{doctorId}/slot/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var apiResponse = await response.Content.ReadAsStringAsync();
            var prescription = JsonSerializer.Deserialize<AppointmentSlotResponse>(apiResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (prescription == null)
            {
                return NotFound();
            }

            // Map dữ liệu từ Response sang Request Model
            AppointmentSlot = new AppointmentSlotRequest
            {
                SlotId = prescription.SlotId,
                ProfileId = prescription.ProfileId,
                Notes = prescription.Notes,
                Medicines = prescription.AppointmentSlotMedicines.Select(m => new AppointmentSlotMedicineRequest
                {
                    MedicineId = m.MedicineId,
                    Dosage = m.Dosage,
                    Instructions = m.Instructions,
                    Quantity = m.Quantity
                }).ToList()
            };

            // Lấy danh sách thuốc
            Medicines = await FetchData<MedicineResponse>(client, "https://localhost:7002/odata/Medicines");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            var client = _clientFactory.CreateClient("UntrustedClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Gửi dữ liệu cập nhật lên API
            var jsonContent = new StringContent(JsonSerializer.Serialize(AppointmentSlot), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"https://localhost:7002/odata/AppointmentSlots/{id}", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                return Page();
            }

            return RedirectToPage("/Prescriptions/Index");
        }

        private async Task<List<T>> FetchData<T>(HttpClient client, string url)
        {
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new List<T>();
            }

            var jsonBytes = await response.Content.ReadAsByteArrayAsync();
            var json = Encoding.UTF8.GetString(jsonBytes);

            try
            {
                return JsonSerializer.Deserialize<ODataResponse<T>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })?.Value ?? new List<T>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ JSON Deserialize Error: {ex.Message}");
                return new List<T>();
            }
        }
    }
}
