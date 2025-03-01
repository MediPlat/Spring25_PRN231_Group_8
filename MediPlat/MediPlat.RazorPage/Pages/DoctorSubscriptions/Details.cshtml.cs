using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using MediPlat.Model.ResponseObject;
using System.Text.Json.Serialization;

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

        public DoctorSubscriptionResponse DoctorSubscription { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound("Subscription ID is required.");
            }

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
                    return Forbid();
                }

                var doctorsJson = await doctorsResponse.Content.ReadAsStringAsync();
                var doctor = JsonSerializer.Deserialize<DoctorResponse>(doctorsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (doctor == null)
                {
                    return Forbid();
                }

                var response = await _httpClient.GetAsync($"https://localhost:7002/odata/DoctorSubscriptions/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return Forbid();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var subscription = JsonSerializer.Deserialize<DoctorSubscriptionResponse>(jsonResponse, 
                    new JsonSerializerOptions{PropertyNameCaseInsensitive = true,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull});
                if (subscription == null || subscription.DoctorId != doctor.Id)
                {
                    return Forbid();
                }

                DoctorSubscription = subscription;
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching the subscription details.");
            }

            return Page();
        }
    }
}
