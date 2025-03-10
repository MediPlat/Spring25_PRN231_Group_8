using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using MediPlat.Model.ResponseObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using static MediPlat.RazorPage.Pages.Experiences.IndexModel;

namespace MediPlat.RazorPage.Pages.Slots
{
    [Authorize(Policy = "DoctorOrPatientPolicy")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor, ILogger<IndexModel> logger)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public List<SlotResponse> Slots { get; set; } = new List<SlotResponse>();
        public int PageSize { get; set; } = 10;
        public int CurrentPage { get; set; } = 1;
        public int TotalItems { get; set; }

        public async Task<IActionResult> OnGetAsync(int page = 1)
        {
            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }

            var client = _clientFactory.CreateClient("UntrustedClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                string apiUrl = $"https://localhost:7002/odata/Slots?$count=true&$top={PageSize}&$skip={(page - 1) * PageSize}&$expand=Doctor,Service";

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"API Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    return StatusCode((int)response.StatusCode, "Error fetching slot list.");
                }

                var apiResponse = await response.Content.ReadAsStringAsync();
                var slotData = JsonSerializer.Deserialize<ODataResponse<SlotResponse>>(apiResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
                Slots = slotData?.Value ?? new List<SlotResponse>();
                TotalItems = slotData?.Count ?? Slots.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching slot data: {ex.Message}");
                return StatusCode(500, "Server error fetching slots.");
            }

            CurrentPage = page;
            return Page();
        }
    }
}
