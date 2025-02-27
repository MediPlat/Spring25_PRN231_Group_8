
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MediPlat.Model.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NuGet.Common;

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
        public DoctorSubscription DoctorSubscription { get; set; } = default!;

        [BindProperty]
        public short AdditionalEnableSlot { get; set; }
        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
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
                var response = await _httpClient.GetAsync($"https://localhost:7002/odata/DoctorSubscriptions/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return Forbid();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                DoctorSubscription = JsonSerializer.Deserialize<DoctorSubscription>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (DoctorSubscription.SubscriptionId == null || DoctorSubscription.SubscriptionId == Guid.Empty)
                {
                    _logger.LogError("SubscriptionId is null after fetching data for DoctorSubscription {Id}", DoctorSubscription.Id);
                    ModelState.AddModelError("", "Failed to load SubscriptionId.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching subscription details for ID {SubscriptionId}", id);
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
            _logger.LogInformation("Received AdditionalEnableSlot: {AdditionalEnableSlot}", AdditionalEnableSlot);

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
                if (DoctorSubscription.SubscriptionId == null || DoctorSubscription.SubscriptionId == Guid.Empty)
                {
                    _logger.LogError("SubscriptionId is null or empty for DoctorSubscription {Id}", DoctorSubscription.Id);
                    ModelState.AddModelError("", "SubscriptionId is required.");
                    return Page();
                }
                short newEnableSlot = (short)(DoctorSubscription.EnableSlot + AdditionalEnableSlot);

                _logger.LogInformation("Before sending request: Current EnableSlot: {EnableSlot}, AdditionalEnableSlot: {AdditionalEnableSlot}, New EnableSlot: {NewEnableSlot}",
                    DoctorSubscription.EnableSlot, AdditionalEnableSlot, newEnableSlot);

                if (newEnableSlot > 1000)
                {
                    ModelState.AddModelError("", "Total EnableSlot cannot exceed 1000.");
                    return Page();
                }
                var requestData = new
                {
                    SubscriptionId = DoctorSubscription.SubscriptionId,
                    EnableSlot = newEnableSlot,
                    UpdateDate = DateTime.UtcNow,
                    Status = DoctorSubscription.Status
                };

                var jsonContent = JsonSerializer.Serialize(requestData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                _logger.LogInformation("Sending request to API: {JsonContent}", jsonContent);

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"https://localhost:7002/odata/DoctorSubscriptions/{DoctorSubscription.Id}", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Update failed: {Error}", errorResponse);
                    ModelState.AddModelError("", $"Update failed: {errorResponse}");
                    return Page();
                }

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating DoctorSubscription for ID {SubscriptionId}", DoctorSubscription.Id);
                ModelState.AddModelError("", "An error occurred while updating the subscription.");
                return Page();
            }
        }
    }
}