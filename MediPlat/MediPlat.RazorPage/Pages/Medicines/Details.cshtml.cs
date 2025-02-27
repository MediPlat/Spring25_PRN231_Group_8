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

namespace MediPlat.RazorPage.Pages.Medicines
{
    [Authorize(Policy = "DoctorOrAdminPolicy")]
    public class DetailsModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(IHttpContextAccessor httpContextAccessor, HttpClient httpClient, ILogger<DetailsModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _logger = logger;
        }

        public Medicine Medicine { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound("Medicine ID is required.");
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
                var response = await _httpClient.GetAsync($"https://localhost:7002/odata/Medicines/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return Forbid();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Medicine = JsonSerializer.Deserialize<Medicine>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (Medicine == null)
                {
                    return NotFound("Medicine không tồn tại.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tải dữ liệu Medicine: {ex.Message}");
                return StatusCode(500, "Lỗi máy chủ khi lấy dữ liệu Medicine.");
            }

            return Page();
        }
    }
}
