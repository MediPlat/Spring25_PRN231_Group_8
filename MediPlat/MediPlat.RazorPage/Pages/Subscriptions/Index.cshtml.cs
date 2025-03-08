using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MediPlat.Model.Model;
using System.Net.Http.Headers;
using System.Text.Json;
using MediPlat.Model.ResponseObject;

namespace MediPlat.RazorPage.Pages.Subscriptions
{
    public class IndexModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory, ILogger<IndexModel> logger)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        public class ODataResponse<T>
        {
            public List<T>? Value { get; set; }
        }

        public IList<SubscriptionResponse> Subscription { get;set; } = new List<SubscriptionResponse>();

        public async Task<IActionResult> OnGetAsync()
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
                string apiUrl = "https://localhost:7002/odata/Subscriptions";
                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"📥 JSON API Response: {apiResponse}");

                    var odataResponse = JsonSerializer.Deserialize<ODataResponse<SubscriptionResponse>>(apiResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    Subscription = odataResponse?.Value ?? new List<SubscriptionResponse>();
                }
                else
                {
                    _logger.LogError($"❌ Lỗi khi tải danh sách Subscription: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi khi tải dữ liệu Subscription: {ex.Message}");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync([FromForm] Guid id)
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

                string apiUrl = $"https://localhost:7002/odata/Subscriptions/{id}";
                var response = await client.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/Subscriptions/Index");
                }
                else
                {
                    _logger.LogError($"Lỗi khi xóa Subscription: {response.ReasonPhrase}");
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
