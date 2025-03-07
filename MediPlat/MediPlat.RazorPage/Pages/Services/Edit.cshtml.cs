using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using static MediPlat.RazorPage.Pages.Experiences.IndexModel;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Microsoft.DotNet.MSIdentity.Shared;

namespace MediPlat.RazorPage.Pages.Services
{
    public class EditModel : PageModel
    {
        [BindProperty]
        public ServiceRequest Service { get; set; } = new();
        public List<SelectListItem> SpecialtyList { get; set; } = new();
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<EditModel> _logger;

        public EditModel(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor, ILogger<EditModel> logger)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private async Task<IActionResult> GetSpecialties()
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
                _logger.LogError($"Lỗi khi lấy danh sách chuyên khoa: {ex.Message}");
                return StatusCode(500, "Lỗi khi tải danh sách chuyên khoa.");
            }
            return null;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
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
                var serviceResponse = await client.GetAsync($"https://localhost:7002/odata/Services/{id}");
                if (!serviceResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"Lỗi khi lấy dịch vụ: {serviceResponse.StatusCode}");
                    return NotFound();
                }
                var jsonResponse = await serviceResponse.Content.ReadAsStringAsync();
                Service = JsonSerializer.Deserialize<ServiceRequest>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                await GetSpecialties();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tải dữ liệu: {ex.Message}");
                return StatusCode(500, "Lỗi khi tải dữ liệu.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            var client = _clientFactory.CreateClient("UntrustedClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jsonContent = new StringContent(JsonSerializer.Serialize(Service), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"https://localhost:7002/odata/Services/{Service.Id}", jsonContent);

            if (response.IsSuccessStatusCode)
                return RedirectToPage("Index");

            await GetSpecialties();
            ModelState.AddModelError("", "Lỗi khi cập nhật dịch vụ.");
            return Page();
        }
    }
}
