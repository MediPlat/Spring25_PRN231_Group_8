using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MediPlat.Model.Model;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;

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
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }

            if (token.StartsWith("Bearer "))
            {
                token = token.Substring("Bearer ".Length);
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using (HttpResponseMessage response = await _httpClient.GetAsync("https://localhost:7002/odata/DoctorSubscriptions"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    DoctorSubscription = JsonConvert.DeserializeObject<List<DoctorSubscription>>(apiResponse) ?? new List<DoctorSubscription>();

                    DoctorFullName = DoctorSubscription.FirstOrDefault()?.Doctor?.FullName ?? "Chưa có thông tin bác sĩ";
                }
            }

            if (DoctorFullName == "Chưa có thông tin bác sĩ")
            {
                var doctorResponse = await _httpClient.GetAsync("https://localhost:7002/odata/Doctors/profile");
                if (doctorResponse.IsSuccessStatusCode)
                {
                    var doctorJson = await doctorResponse.Content.ReadAsStringAsync();
                    var doctor = JsonConvert.DeserializeObject<Doctor>(doctorJson);
                    DoctorFullName = doctor?.FullName ?? "Chưa có thông tin bác sĩ";
                }
            }

            ViewData["Title"] = $"Gói đăng ký của bác sĩ {DoctorFullName}";

            return Page();
        }
    }
}