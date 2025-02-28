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
                var response = await _httpClient.GetAsync($"https://localhost:7002/odata/DoctorSubscriptions/{id}?$expand=Doctor,Subscription");
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return Forbid();
                }

                DoctorSubscription = JsonSerializer.Deserialize<DoctorSubscription>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching the subscription details.");
            }

            return Page();
        }
    }
}