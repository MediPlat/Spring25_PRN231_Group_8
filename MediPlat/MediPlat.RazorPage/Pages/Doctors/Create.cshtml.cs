using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MediPlat.Model.Model;
using System.Net.Http.Headers;
using MediPlat.Model.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MediPlat.RazorPage.Pages.Doctors
{
    public class CreateModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<IndexModel> _logger;
        public CreateModel(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory, ILogger<IndexModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        [BindProperty]
        public DoctorSchema Doctor { get; set; } = new DoctorSchema();

        public IActionResult OnGet()
        {
            return Page();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostCreateAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Lỗi dữ liệu không hợp lệ!");
                    return new JsonResult(new { success = false, message = "Dữ liệu không hợp lệ!" });
                }

                var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
                if (string.IsNullOrEmpty(token))
                {
                    return new JsonResult(new { success = false, message = "Token không hợp lệ, vui lòng đăng nhập lại!" });
                }

                var client = _clientFactory.CreateClient("UntrustedClient");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.PostAsJsonAsync("odata/Doctors/create_doctor", Doctor);

                if (response.IsSuccessStatusCode)
                {
                    return new JsonResult(new { success = true, message = "Thêm mới bác sĩ thành công!" });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return new JsonResult(new { success = false, message = $"Lỗi: {error}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi server: {ex.Message}");
                return new JsonResult(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        public IActionResult OnGetCreateModal()
        {
            Doctor = new DoctorSchema();
            return Partial("_CreateDoctorPartial", this); // Trả về trang Create luôn
        }

    }
}
