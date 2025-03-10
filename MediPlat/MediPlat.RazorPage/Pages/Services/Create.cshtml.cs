using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using static MediPlat.RazorPage.Pages.Experiences.IndexModel;

namespace MediPlat.RazorPage.Pages.Services
{
    [Authorize(Policy = "AdminPolicy")]
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CreateModel> _logger;

        [BindProperty]
        public ServiceRequest Service { get; set; } = new();
        public List<SelectListItem> SpecialtyList { get; set; } = [];

        public CreateModel(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor, ILogger<CreateModel> logger)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

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
                var specialtyResponse = await client.GetAsync("https://localhost:7002/odata/Specialties");

                if (!specialtyResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"Lỗi khi lấy danh sách chuyên khoa: {specialtyResponse.StatusCode}");
                    return StatusCode((int)specialtyResponse.StatusCode, "Không thể tải danh sách chuyên khoa.");
                }

                var specialtyJson = await specialtyResponse.Content.ReadAsStringAsync();
                var specialties = JsonSerializer.Deserialize<ODataResponse<SpecialtyResponse>>(specialtyJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                SpecialtyList = specialties?.Value?.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList() ?? new List<SelectListItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tải danh sách chuyên khoa: {ex.Message}");
                return StatusCode(500, "Lỗi khi tải danh sách chuyên khoa.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            var client = _clientFactory.CreateClient("UntrustedClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jsonContent = new StringContent(JsonSerializer.Serialize(Service), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7002/odata/Services", jsonContent);

            if (response.IsSuccessStatusCode)
                return RedirectToPage("./Index");

            ModelState.AddModelError("", "Lỗi khi thêm dịch vụ.");
            return Page();
        }
    }
}
