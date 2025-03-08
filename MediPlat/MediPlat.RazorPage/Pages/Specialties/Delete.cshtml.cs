using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MediPlat.Model.ResponseObject;
using System.Text.Json;

namespace MediPlat.RazorPage.Pages.Specialties
{
    [Authorize(Policy = "AdminPolicy")]
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
        public SpecialtyResponse Specialty { get; set; } = new SpecialtyResponse();

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
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
                var response = await client.GetAsync($"https://localhost:7002/odata/Specialties/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Specialty = JsonSerializer.Deserialize<SpecialtyResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (Specialty == null)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"L?i khi t?i d? li?u chuyên khoa: {ex.Message}");
                return StatusCode(500, "L?i khi t?i d? li?u.");
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
                var response = await client.DeleteAsync($"https://localhost:7002/odata/Specialties/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Không th? xóa chuyên khoa. Chi ti?t l?i: {errorResponse}");
                    return Page();
                }

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"L?i khi xóa chuyên khoa: {ex.Message}");
                ModelState.AddModelError("", "L?i khi xóa chuyên khoa. Vui lòng th? l?i.");
                return Page();
            }
        }
    }
}
