using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using MediPlat.Model.ResponseObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static MediPlat.RazorPage.Pages.Experiences.IndexModel;
using static MediPlat.RazorPage.Pages.Subscriptions.IndexModel;

namespace MediPlat.RazorPage.Pages.Medicines
{
    [Authorize(Policy = "DoctorOrAdminPolicy")]
    public class IndexModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory, ILogger<IndexModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public List<MedicineResponse> Medicines { get; set; } = new List<MedicineResponse>();
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
                string apiUrl = $"https://localhost:7002/odata/Medicines?$count=true&$top={PageSize}&$skip={(page - 1) * PageSize}";
                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var medicineData = JsonSerializer.Deserialize<ODataResponse<MedicineResponse>>(apiResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    Medicines = medicineData?.Value ?? new List<MedicineResponse>();
                    TotalItems = medicineData?.Count ?? Medicines.Count;
                }
                else
                {
                    _logger.LogError($"❌ Lỗi khi tải danh sách thuốc: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi khi tải dữ liệu Medicines: {ex.Message}");
            }

            CurrentPage = page;
            return Page();
        }
    }
}