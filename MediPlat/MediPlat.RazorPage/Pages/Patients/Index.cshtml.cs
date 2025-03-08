using MediPlat.Model.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace MediPlat.RazorPage.Pages.Patients
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IndexModel(IHttpContextAccessor httpContextAccessor, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public IList<Patient> Patient { get; set; } = [];

        public async Task OnGetAsync()
        {
            //Patient = await _context.Patients.ToListAsync();
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("⚠️ Không tìm thấy token ở Index.cshtml.cs của Patient, chuyển hướng đến trang login...");
                RedirectToPage("/Auth/Login");
            }
            if (token.StartsWith("Bearer "))
            {
                token = token.Substring("Bearer ".Length);
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using (HttpResponseMessage response = await _httpClient.GetAsync("https://localhost:7002/odata/Patients"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(apiResponse); // Parse JSON response
                    var patientsArray = jsonObject["value"]?.ToString(); // Extract "value" array

                    if (!string.IsNullOrEmpty(patientsArray))
                    {
                        Patient = JsonConvert.DeserializeObject<List<Patient>>(patientsArray);
                    }
                }

            }
        }
    }
}
