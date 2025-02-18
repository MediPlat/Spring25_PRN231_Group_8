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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            var authCookie = HttpContext.Request.Cookies[".AspNetCore.Cookies"];
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("⚠️ Không tìm thấy token ở Index.cshtml.cs của DoctorSubscription, chuyển hướng đến trang login...");
                return RedirectToPage("/Auth/Login");
            }

            if (token.StartsWith("Bearer "))
            {
                token = token.Substring("Bearer ".Length);
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using (HttpResponseMessage response = await _httpClient.GetAsync("https://localhost:7002/odata/DoctorSubscriptions"))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    DoctorSubscription = JsonConvert.DeserializeObject<List<DoctorSubscription>>(apiResponse);

                    if (DoctorSubscription != null && DoctorSubscription.Any())
                    {
                        DoctorFullName = DoctorSubscription.First().Doctor.FullName;
                    }
                    else
                    {
                        Console.WriteLine("⚠️ Không có gói đăng ký nào cho bác sĩ này.");
                        var doctorResponse = await _httpClient.GetAsync("https://localhost:7002/odata/Doctors/profile");
                        if (doctorResponse.IsSuccessStatusCode)
                        {
                            var doctorJson = await doctorResponse.Content.ReadAsStringAsync();
                            var doctor = JsonConvert.DeserializeObject<Doctor>(doctorJson);

                            if (doctor != null)
                            {
                                DoctorFullName = doctor.FullName;
                            }
                        }
                    }
                }

            }
            ViewData["Title"] = "Gói đăng ký của bác sĩ " + (string.IsNullOrEmpty(DoctorFullName) ? "Không có tên" : DoctorFullName);
            return Page();
        }
    }
}