using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MediPlat.Model.Model;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using static MediPlat.RazorPage.Pages.Experiences.IndexModel;
using MediPlat.Model.Schema;

namespace MediPlat.RazorPage.Pages.Doctors
{
    [Authorize(Policy = "DoctorOrAdminPolicy")]
    public class DoctorModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<IndexModel> _logger;

    
        public DoctorModel(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory, ILogger<IndexModel> logger)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public IList<Doctor> DoctorList { get; set; } = new List<Doctor>();
        public int PageSize { get; set; } = 10;
        public int CurrentPage { get; set; } = 1;
        public int TotalItems { get; set; }

        public async Task<IActionResult> OnGetAsync()
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
                string apiUrl = "https://localhost:7002/odata/Doctors/all_doctor";
                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"📥 JSON API Response: {apiResponse}");

                    DoctorList = JsonSerializer.Deserialize<List<Doctor>>(apiResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Doctor>();


                }
                else
                {
                    _logger.LogError($"❌ Lỗi khi tải danh sách bác sĩ: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi khi tải dữ liệu DoctorList: {ex.Message}");
            }

            return Page();
        }

        

        public async Task<IActionResult> OnPostToggleStatusAsync(Guid id, string status)
        {
            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }

            var client = _clientFactory.CreateClient("UntrustedClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var apiUrl = $"https://localhost:7002/odata/Doctors/banned_unbanned?id={id}";

            var request = new HttpRequestMessage(HttpMethod.Patch, apiUrl);

            try
            {
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Doctor ID {id} status updated successfully.");
                }
                else
                {
                    _logger.LogError($"Failed to update doctor status. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception while calling API: {ex.Message}");
            }

            return RedirectToPage();
        }

    }
}
