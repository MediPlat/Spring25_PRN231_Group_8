using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MediPlat.Model.Model;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using MediPlat.Model.ResponseObject;

namespace MediPlat.RazorPage.Pages.DoctorSubscriptions
{
    [Authorize(Policy = "DoctorPolicy")]
    public class IndexModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;

        public IndexModel(IHttpContextAccessor httpContextAccessor, HttpClient httpClient)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
        }

        public IList<DoctorSubscription> DoctorSubscription { get; set; } = new List<DoctorSubscription>();
        public string DoctorFullName { get; set; } = "Chưa có thông tin bác sĩ";
        public async Task<IActionResult> OnGetAsync()
        {
            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using (HttpResponseMessage response = await _httpClient.GetAsync("https://localhost:7002/odata/DoctorSubscriptions?$expand=Doctor,Subscription"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonDocument.Parse(apiResponse);
                    var root = jsonDocument.RootElement;

                    if (root.TryGetProperty("value", out JsonElement valueElement))
                    {
                        DoctorSubscriptionResponses = JsonSerializer.Deserialize<List<DoctorSubscriptionResponse>>(valueElement.GetRawText(), new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        }) ?? new List<DoctorSubscriptionResponse>();
                    }
                    else
                    {
                        DoctorSubscriptionResponses = new List<DoctorSubscriptionResponse>();
                    }
                }
            }

            ViewData["Title"] = "Danh sách gói đăng ký của bác sĩ";
            return Page();
        }
        public IList<DoctorSubscriptionResponse> DoctorSubscriptionResponses { get; set; } = new List<DoctorSubscriptionResponse>();
    }
}