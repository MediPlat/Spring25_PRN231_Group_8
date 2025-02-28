﻿using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using static MediPlat.RazorPage.Pages.Experiences.DetailsModel;

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
        public ExperienceRequest Experience { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var specialtiesResponse = await _httpClient.GetAsync("https://localhost:7002/odata/Specialties");
                if (specialtiesResponse.IsSuccessStatusCode)
                {
                    var specialtiesJson = await specialtiesResponse.Content.ReadAsStringAsync();
                    var odataResponse = JsonSerializer.Deserialize<ODataResponse<Specialty>>(specialtiesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    var specialties = odataResponse?.Value ?? new List<Specialty>(); 
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
                        Experience = new ExperienceRequest { DoctorId = doctor?.Id ?? Guid.Empty };
                    }
                    if (Experience.DoctorId == Guid.Empty || Experience.SpecialtyId == Guid.Empty)
                    {
                        ModelState.AddModelError("", "Vui lòng chọn Bác sĩ và Chuyên khoa hợp lệ.");
                        return Page();
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

            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var checkResponse = await _httpClient.GetAsync($"https://localhost:7002/odata/Experiences?$filter=doctorId eq '{Experience.DoctorId}' and specialtyId eq '{Experience.SpecialtyId}'");
                if (checkResponse.IsSuccessStatusCode)
                {
                    var existingExperiencesJson = await checkResponse.Content.ReadAsStringAsync();
                    var jsonDocument = JsonNode.Parse(existingExperiencesJson);
                    var experiencesArray = jsonDocument?["value"]?.AsArray();

                    if (experiencesArray != null && experiencesArray.Count > 0)
                    {
                        ModelState.AddModelError("", "Bác sĩ này đã có Experience với chuyên khoa này.");
                        return Page();
                    }
                }

                var jsonContent = JsonSerializer.Serialize(Experience, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("https://localhost:7002/odata/Experiences", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    var errorMessage = JsonSerializer.Deserialize<JsonElement>(errorResponse).GetProperty("message").GetString();
                    ModelState.AddModelError("", $"❌ {errorMessage}");
                    return Page();
                }

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tạo Experience: {ex.Message}");
                ModelState.AddModelError("", "🚨 Có lỗi xảy ra. Vui lòng thử lại sau.");
                return Page();
            }
        }
    }
}
