using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using MediPlat.Model.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MediPlat.RazorPage.Pages.Experiences
{
    [Authorize(Policy = "DoctorOrAdminorPatientPolicy")]
    public class IndexModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IHttpContextAccessor httpContextAccessor, HttpClient httpClient, ILogger<IndexModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _logger = logger;
        }

        public IList<Experience> Experience { get; set; } = new List<Experience>();
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

            try
            {
                using (HttpResponseMessage response = await _httpClient.GetAsync("https://localhost:7002/odata/Experiences?$expand=Doctor,Specialty"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse = await response.Content.ReadAsStringAsync();
                        Experience = JsonSerializer.Deserialize<List<Experience>>(apiResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Experience>();
                    }
                    else
                    {
                        _logger.LogError("Lỗi khi tải danh sách Experience: " + response.ReasonPhrase);
                    }
                }

                DoctorFullName = Experience.FirstOrDefault()?.Doctor?.FullName ?? "Chưa có thông tin bác sĩ";

                if (DoctorFullName == "Chưa có thông tin bác sĩ")
                {
                    var doctorResponse = await _httpClient.GetAsync("https://localhost:7002/odata/Doctors/profile");
                    if (doctorResponse.IsSuccessStatusCode)
                    {
                        var doctorJson = await doctorResponse.Content.ReadAsStringAsync();
                        var doctor = JsonSerializer.Deserialize<Doctor>(doctorJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        DoctorFullName = doctor?.FullName ?? "Chưa có thông tin bác sĩ";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tải dữ liệu Experience: {ex.Message}");
            }

            ViewData["Title"] = $"Danh sách Experience của {DoctorFullName}";
            return Page();
        }
    }
}
