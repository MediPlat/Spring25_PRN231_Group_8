using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using MediPlat.Model.ResponseObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MediPlat.RazorPage.Pages.Services
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

        public IList<ServiceResponse> Services { get; set; } = new List<ServiceResponse>();
        public int PageSize { get; set; } = 10;
        public int CurrentPage { get; set; } = 1;
        public int TotalItems { get; set; }

        public async Task<IActionResult> OnGetAsync(int skip = 1)
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
                string apiUrl = $"https://localhost:7002/odata/Services?$top={PageSize}&$skip={(skip - 1) * PageSize}&$count=true";
                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonNode.Parse(apiResponse);

                    var medicinesArray = jsonDocument?["value"]?.AsArray();
                    var totalCount = jsonDocument?["@odata.count"]?.GetValue<int>() ?? 0;
                    TotalItems = totalCount;

                    if (medicinesArray != null)
                    {
                        Services = medicinesArray.Deserialize<List<ServiceResponse>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ServiceResponse>();
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
                _logger.LogError($"❌ Lỗi khi tải dữ liệu Services: {ex.Message}");
            }

            CurrentPage = skip;
            return Page();
        }
    }
}