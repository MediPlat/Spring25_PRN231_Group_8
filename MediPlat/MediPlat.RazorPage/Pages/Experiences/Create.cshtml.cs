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

namespace MediPlat.RazorPage.Pages.Experiences
{
    [Authorize(Policy = "DoctorPolicy")]
    public class CreateModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory, ILogger<CreateModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        [BindProperty]
        public ExperienceRequest Experience { get; set; } = new ExperienceRequest();

        public List<SelectListItem> SpecialtyList { get; set; } = new List<SelectListItem>();

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

                SpecialtyList = specialties?.Value?.ToList()?.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList() ?? new List<SelectListItem>();

                var doctorResponse = await client.GetAsync("https://localhost:7002/odata/Doctors/profile");

                if (!doctorResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"Lỗi khi lấy hồ sơ bác sĩ: {doctorResponse.StatusCode}");
                    return StatusCode((int)doctorResponse.StatusCode, "Không thể tải thông tin bác sĩ.");
                }

                var doctorJson = await doctorResponse.Content.ReadAsStringAsync();
                var doctor = JsonSerializer.Deserialize<DoctorResponse>(doctorJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (doctor == null)
                {
                    return StatusCode(500, "Lỗi khi tải thông tin bác sĩ.");
                }

                Experience.DoctorId = doctor.Id;
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi khi tải dữ liệu.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }
            var client = _clientFactory.CreateClient("UntrustedClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var jsonContent = JsonSerializer.Serialize(Experience);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://localhost:7002/odata/Experiences", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Lỗi khi tạo Experience: {errorResponse}");

                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        ModelState.AddModelError("", $"Lỗi khi tạo Experience: {errorResponse}");
                        return Page();
                    }

                    return StatusCode((int)response.StatusCode, "Lỗi khi tạo Experience.");
                }

                _logger.LogInformation($"Experience '{Experience.Title}' đã được tạo thành công.");
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tạo Experience: {ex.Message}");
                return StatusCode(500, "Lỗi máy chủ khi tạo Experience.");
            }
        }
    }
}
