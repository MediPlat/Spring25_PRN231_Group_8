using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MediPlat.Model.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MediPlat.RazorPage.Pages.Experiences
{
    [Authorize(Policy = "DoctorPolicy")]
    public class CreateModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(IHttpContextAccessor httpContextAccessor, HttpClient httpClient, ILogger<CreateModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _logger = logger;
        }

        [BindProperty]
        public Experience Experience { get; set; } = default!;

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
                var specialtiesResponse = await _httpClient.GetAsync("https://localhost:7002/odata/Specialties");
                if (specialtiesResponse.IsSuccessStatusCode)
                {
                    var specialtiesJson = await specialtiesResponse.Content.ReadAsStringAsync();
                    var specialties = JsonSerializer.Deserialize<List<Specialty>>(specialtiesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    ViewData["SpecialtyId"] = new SelectList(specialties, "Id", "Name");
                }

                var doctorResponse = await _httpClient.GetAsync("https://localhost:7002/odata/Doctors/profile");
                if (doctorResponse.IsSuccessStatusCode)
                {
                    var doctorJson = await doctorResponse.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Doctor API Response: {doctorJson}");

                    var doctor = JsonSerializer.Deserialize<Doctor>(doctorJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (Experience == null)
                    {
                        Experience = new Experience();
                    }
                    if (doctor != null)
                    {
                        ViewData["DoctorId"] = new SelectList(new List<Doctor> { doctor }, "Id", "FullName");
                        Experience.DoctorId = doctor.Id;
                    }
                    else
                    {
                        _logger.LogError("Doctor object is null after deserialization!");
                    }
                }
                else
                {
                    _logger.LogError($"Failed to fetch doctor. Status Code: {doctorResponse.StatusCode}, Response: {await doctorResponse.Content.ReadAsStringAsync()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tải dữ liệu: {ex.Message}");
            }

            return Page();
        }
    

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

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
                var jsonContent = JsonSerializer.Serialize(Experience, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var checkResponse = await _httpClient.GetAsync($"https://localhost:7002/odata/Experiences?$filter=doctorId eq {Experience.DoctorId} and specialtyId eq {Experience.SpecialtyId}");
                if (checkResponse.IsSuccessStatusCode)
                {
                    var existingExperiencesJson = await checkResponse.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(existingExperiencesJson) && existingExperiencesJson.Contains("value"))
                    {
                        ModelState.AddModelError("", "Bác sĩ này đã có Experience với chuyên khoa này.");
                        return Page();
                    }
                }
                var response = await _httpClient.PostAsync("https://localhost:7002/odata/Experiences", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Không thể tạo Experience. Chi tiết lỗi: {errorResponse}");
                    return Page();
                }

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi tạo Experience. Vui lòng thử lại.");
                return Page();
            }
        }
    }
}
