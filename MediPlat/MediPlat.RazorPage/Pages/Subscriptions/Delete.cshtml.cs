using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MediPlat.Model.Model;
using System.Net.Http.Headers;

namespace MediPlat.RazorPage.Pages.Subscriptions
{
    public class DeleteModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<IndexModel> _logger;

        public DeleteModel(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory, ILogger<IndexModel> logger)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [BindProperty]
        public Subscription Subscription { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            return Page();
        }


        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnPostDeleteAsync(Guid id)

        {
            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(token))
            {
                return new JsonResult(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var client = _clientFactory.CreateClient("UntrustedClient");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string apiUrl = $"https://localhost:7002/odata/Subscriptions({id})";
                var response = await client.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    return new JsonResult(new { success = true });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Lỗi chi tiết: {error}");
                    return new JsonResult(new { success = false });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi xóa Subscription: {ex.Message}");
                return new JsonResult(new { success = false });
            }
        }
    }
}
