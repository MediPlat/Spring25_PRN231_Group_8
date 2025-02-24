using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using MediPlat.Model.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MediPlat.RazorPage.Pages.Experiences
{
    [Authorize(Policy = "DoctorPolicy")]
    public class DeleteModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(IHttpContextAccessor httpContextAccessor, HttpClient httpClient, ILogger<DeleteModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _logger = logger;
        }

        [BindProperty]
        public Experience Experience { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound("Experience ID is required.");
            }

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
                var response = await _httpClient.GetAsync($"https://localhost:7002/odata/Experiences/{id}?$expand=Doctor,Specialty");
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound("Experience không tồn tại.");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Experience = JsonSerializer.Deserialize<Experience>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (Experience == null)
                {
                    return NotFound("Experience không tồn tại.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tải dữ liệu Experience: {ex.Message}");
                return StatusCode(500, "Lỗi máy chủ khi lấy dữ liệu Experience.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound("Experience ID is required.");
            }

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
                var response = await _httpClient.DeleteAsync($"https://localhost:7002/odata/Experiences/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Không thể xóa Experience. Chi tiết lỗi: {errorResponse}");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi xóa Experience: {ex.Message}");
                ModelState.AddModelError("", "Lỗi khi xóa Experience. Vui lòng thử lại.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
