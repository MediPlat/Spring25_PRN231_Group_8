using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using MediPlat.Model.ResponseObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MediPlat.RazorPage.Pages.Medicines
{
    [Authorize(Policy = "DoctorOrAdminPolicy")]
    public class IndexModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IHttpContextAccessor httpContextAccessor, HttpClient httpClient, ILogger<IndexModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _logger = logger;
        }

        public IList<MedicineResponse> Medicines { get; set; } = new List<MedicineResponse>();
        public int PageSize { get; set; } = 10;
        public int CurrentPage { get; set; } = 1;
        public int TotalItems { get; set; }  // Lấy từ danh sách thuốc trả về

        public async Task<IActionResult> OnGetAsync(int page = 1)
        {
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
                string apiUrl = $"https://localhost:7002/odata/Medicines?$top={PageSize}&$skip={(page - 1) * PageSize}";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"📥 JSON API Response: {apiResponse}"); // Debug JSON từ API

                    var jsonDocument = JsonNode.Parse(apiResponse);
                    var medicinesArray = jsonDocument?["value"]?.AsArray();

                    if (medicinesArray != null)
                    {
                        Medicines = medicinesArray.Deserialize<List<MedicineResponse>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<MedicineResponse>();
                        TotalItems = Medicines.Count; // Lấy tổng số thuốc từ danh sách trả về
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ API không trả về danh sách thuốc.");
                    }
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