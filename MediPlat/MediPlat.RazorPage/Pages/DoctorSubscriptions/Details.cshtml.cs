using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using MediPlat.Model.Model;

namespace MediPlat.RazorPage.Pages.DoctorSubscriptions
{
    [Authorize(Policy = "DoctorPolicy")]
    public class DetailsModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(IHttpContextAccessor httpContextAccessor, HttpClient httpClient, ILogger<DetailsModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _logger = logger;
        }

        public DoctorSubscription DoctorSubscription { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Details page accessed without an ID.");
                return NotFound("Subscription ID is required.");
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
                var response = await _httpClient.GetAsync($"https://localhost:7002/odata/DoctorSubscriptions/{id}?$expand=Doctor,Subscription");
                var jsonResponse = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("API Response: {JsonResponse}", jsonResponse);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Doctor attempted to access an unauthorized or non-existent subscription {SubscriptionId}.", id);
                    return Forbid();
                }

                DoctorSubscription = JsonSerializer.Deserialize<DoctorSubscription>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Kiểm tra xem Doctor và Subscription có dữ liệu không
                if (DoctorSubscription?.Doctor == null)
                {
                    _logger.LogWarning("Doctor information is missing for subscription ID {SubscriptionId}", id);
                }
                if (DoctorSubscription?.Subscription == null)
                {
                    _logger.LogWarning("Subscription information is missing for subscription ID {SubscriptionId}", id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching subscription details for ID {SubscriptionId}", id);
                return StatusCode(500, "An error occurred while fetching the subscription details.");
            }

            return Page();
        }
    }
}