using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using MediPlat.Model.Model;
using MediPlat.Model.ResponseObject;
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
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory, ILogger<DetailsModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
            _logger = logger;
        }
        [BindProperty]
        public string? ReturnUrl { get; set; }
        public MedicineResponse Medicine { get; set; } = default!;
        public bool IsAdmin { get; private set; } = false;
        public async Task<IActionResult> OnGetAsync(Guid? id, string? returnUrl)
        {
            if (id == null)
            {
                return NotFound("Medicine ID is required.");
            }

            ReturnUrl = returnUrl;

            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }

            var client = _clientFactory.CreateClient("UntrustedClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            IsAdmin = userRole == "Admin";
            try
            {
                var response = await client.GetAsync($"https://localhost:7002/odata/Medicines/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return Forbid();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Medicine = JsonSerializer.Deserialize<MedicineResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

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
