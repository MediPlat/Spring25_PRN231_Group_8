using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using System.Net.Http.Headers;
using MediPlat.Model.Schema;

namespace MediPlat.RazorPage.Pages.Subscriptions
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

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public SubscriptionRequest Subscription { get; set; } = new SubscriptionRequest()!;

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

                var response = await client.PostAsJsonAsync("odata/Subscriptions", Subscription);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/Subscriptions/Index");
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
            Subscription = new SubscriptionRequest();
            return Partial("_CreateSubscriptionPartial", this); // Trả về trang Create luôn
        }
    }
}
