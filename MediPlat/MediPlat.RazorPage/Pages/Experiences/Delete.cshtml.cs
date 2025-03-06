using System.Net.Http.Headers;
using System.Text.Json;
using MediPlat.Model.ResponseObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MediPlat.RazorPage.Pages.Experiences
{
    [Authorize(Policy = "DoctorPolicy")]
    public class DeleteModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory, ILogger<DeleteModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        [BindProperty]
        public ExperienceResponse? Experience { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound("Experience ID không hợp lệ.");
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
                string apiUrl = $"https://localhost:7002/odata/Experiences/{id}?$expand=Doctor,Specialty";
                _logger.LogInformation($"Fetching Experience details before deletion: {apiUrl}");

                var response = await client.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"API Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");

                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return NotFound("Experience không tồn tại.");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return RedirectToPage("/Auth/Login");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        return Forbid();
                    }
                    return StatusCode((int)response.StatusCode, "Lỗi khi tải chi tiết Experience.");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Experience = JsonSerializer.Deserialize<ExperienceResponse>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (Experience == null)
                {
                    _logger.LogWarning($"Experience với ID {id} không có dữ liệu hợp lệ.");
                    return NotFound("Experience không có dữ liệu hợp lệ.");
                }

                _logger.LogInformation($"Loaded Experience for deletion: ID={Experience.Id}, Title={Experience.Title}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tải Experience: {ex.Message}");
                return StatusCode(500, "Lỗi máy chủ khi tải chi tiết Experience.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
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
                string apiUrl = $"https://localhost:7002/odata/Experiences/{id}";
                _logger.LogInformation($"Attempting to delete Experience: {apiUrl}");

                var response = await client.DeleteAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Lỗi khi xóa Experience: {errorResponse}");

                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return NotFound("Experience không tồn tại.");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        return Forbid();
                    }
                    return StatusCode((int)response.StatusCode, "Lỗi khi xóa Experience.");
                }

                _logger.LogInformation($"Experience với ID {id} đã được xóa thành công.");
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi xóa Experience: {ex.Message}");
                return StatusCode(500, "Lỗi máy chủ khi xóa Experience.");
            }
        }
    }
}
