using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MediPlat.Model.RequestObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MediPlat.RazorPage.Pages.Specialties
{
    [Authorize(Policy = "AdminPolicy")]
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
        public SpecialtyRequest Specialty { get; set; } = new SpecialtyRequest();

        public async Task<IActionResult> OnGetAsync()
        {
            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }
            var client = _clientFactory.CreateClient("UntrustedClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            await Task.CompletedTask;
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
                var medicineRequest = new SpecialtyRequest
                {
                    Name = Specialty.Name,
                    Description = Specialty.Description,
                };

                var jsonContent = JsonSerializer.Serialize(medicineRequest, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://localhost:7002/odata/Specialties", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();

                    ModelState.AddModelError("", $"Không thể tạo chuyên khoa. API phản hồi: {errorResponse}");
                    return Page();
                }
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi tạo chuyên khoa. Vui lòng thử lại.");
                return Page();
            }
        }
    }
}
