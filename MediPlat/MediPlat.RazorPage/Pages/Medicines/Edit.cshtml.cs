using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MediPlat.RazorPage.Pages.Medicines
{
    [Authorize(Policy = "AdminPolicy")]
    public class EditModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly ILogger<EditModel> _logger;

        public EditModel(IHttpContextAccessor httpContextAccessor, HttpClient httpClient, ILogger<EditModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _logger = logger;
        }

        [BindProperty]
        public MedicineRequest Medicine { get; set; } = new MedicineRequest();

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var token = GetBearerToken();
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7002/odata/Medicines/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var medicineResponse = JsonSerializer.Deserialize<MedicineResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (medicineResponse != null)
                {
                    Medicine = new MedicineRequest
                    {
                        Name = medicineResponse.Name,
                        DosageForm = medicineResponse.DosageForm,
                        Strength = medicineResponse.Strength,
                        SideEffects = medicineResponse.SideEffects,
                        Status = medicineResponse.Status
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tải dữ liệu thuốc: {ex.Message}");
                return StatusCode(500, "Lỗi khi tải dữ liệu.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var token = GetBearerToken();
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var jsonContent = JsonSerializer.Serialize(Medicine, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"https://localhost:7002/odata/Medicines/{id}", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Không thể cập nhật thuốc. Chi tiết lỗi: {errorResponse}");
                    return Page();
                }

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi cập nhật thuốc: {ex.Message}");
                ModelState.AddModelError("", "Lỗi khi cập nhật thuốc. Vui lòng thử lại.");
                return Page();
            }
        }

        private string GetBearerToken()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            return string.IsNullOrEmpty(token) || !token.StartsWith("Bearer ") ? token : token.Substring("Bearer ".Length);
        }
    }
}
