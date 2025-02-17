using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MediPlat.Model.Model;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MediPlat.RazorPage.Pages.DoctorSubscriptions
{
    public class IndexModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        public IList<DoctorSubscription> DoctorSubscription { get; set; } = new List<DoctorSubscription>();

        public IndexModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = new HttpClient();
        }

        public async Task OnGetAsync()
        {
            // ✅ Lấy JWT Token từ HttpContext
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("⚠️ Không tìm thấy token, có thể bạn chưa đăng nhập!");
                return;
            }

            // ✅ Đính kèm token vào Header khi gọi API
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using (HttpResponseMessage response = await _httpClient.GetAsync("https://localhost:7002/odata/DoctorSubscriptions"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("✅ API Response: " + apiResponse);
                    var result = JsonConvert.DeserializeObject<ODataResponse<List<DoctorSubscription>>>(apiResponse);
                    if (result != null)
                    {
                        DoctorSubscription = result.Value;
                    }
                }
                else
                {
                    Console.WriteLine($"❌ API call failed with status: {response.StatusCode}");
                }
            }
        }

        public class ODataResponse<T>
        {
            [JsonProperty("value")]
            public T Value { get; set; }
        }
    }
}