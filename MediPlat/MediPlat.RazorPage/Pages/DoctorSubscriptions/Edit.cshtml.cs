using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace MediPlat.RazorPage.Pages.DoctorSubscriptions
{
    [Authorize(Policy = "DoctorPolicy")]
    public class EditModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly ILogger<EditModel> _logger;

        public EditModel(IHttpContextAccessor httpContextAccessor, HttpClient httpClient, ILogger<EditModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _logger = logger;
        }

        [BindProperty]
        public DoctorSubscriptionResponse DoctorSubscription { get; set; } = default!;
        [BindProperty]
        public DoctorSubscriptionRequest DoctorSubscriptionss { get; set; } = default!;

        [BindProperty]
        public short AdditionalEnableSlot { get; set; }
        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7002/odata/DoctorSubscriptions/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return Forbid();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var doctorSubscriptionResponse = JsonSerializer.Deserialize<DoctorSubscriptionResponse>(
                    jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (doctorSubscriptionResponse == null || doctorSubscriptionResponse.SubscriptionId == Guid.Empty)
                {
                    ModelState.AddModelError("", "Failed to load SubscriptionId.");
                    return Page();
                }

                DoctorSubscription = doctorSubscriptionResponse;

                DoctorSubscriptionss = new DoctorSubscriptionRequest
                {
                    SubscriptionId = doctorSubscriptionResponse.SubscriptionId,
                    EnableSlot = doctorSubscriptionResponse.EnableSlot ?? (short)0,
                    Status = doctorSubscriptionResponse.Status ?? "Active",
                    DoctorId = doctorSubscriptionResponse.DoctorId
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching the subscription details.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (DoctorSubscriptionss.SubscriptionId == Guid.Empty)
            {
                ModelState.AddModelError("", "SubscriptionId is required.");
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
                if (DoctorSubscriptionss.SubscriptionId == Guid.Empty)
                {
                    ModelState.AddModelError("", "SubscriptionId is required.");
                    return Page();
                }

                short newEnableSlot = (short)(DoctorSubscriptionss.EnableSlot + AdditionalEnableSlot);
                if (newEnableSlot > 1000)
                {
                    ModelState.AddModelError("", "Total EnableSlot cannot exceed 1000.");
                    return Page();
                }

                var requestData = new DoctorSubscriptionRequest
                {
                    SubscriptionId = DoctorSubscriptionss.SubscriptionId,
                    EnableSlot = newEnableSlot,
                    UpdateDate = DateTime.UtcNow,
                    Status = DoctorSubscriptionss.Status,
                    DoctorId = DoctorSubscriptionss.DoctorId
                };

                var jsonContent = JsonSerializer.Serialize(requestData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"https://localhost:7002/odata/DoctorSubscriptions/{DoctorSubscription.Id}", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Update failed: {errorResponse}");
                    return Page();
                }

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the subscription.");
                return Page();
            }
        }
    }
}