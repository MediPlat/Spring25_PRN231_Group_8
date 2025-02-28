using System.Net.Http.Headers;
using System.Text.Json;
using MediPlat.Model.ResponseObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public List<ExperienceResponse> Experiences { get; set; } = new List<ExperienceResponse>();
        public string DoctorFullName { get; set; } = "Chưa có thông tin bác sĩ";

        public int PageSize { get; set; } = 5;
        public int CurrentPage { get; set; } = 1;
        public int TotalItems { get; set; }
        
        public async Task<IActionResult> OnGetAsync(int page = 1)
        {
            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                string apiUrl = $"https://localhost:7002/odata/Experiences?$top={PageSize}&$skip={(page - 1) * PageSize}&$expand=Doctor,Specialty";
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var experienceData = JsonSerializer.Deserialize<JsonElement>(apiResponse);

                    if (experienceData.TryGetProperty("value", out JsonElement experiencesJson))
                    {
                        var experiencesList = new List<ExperienceResponse>();

                        foreach (var element in experiencesJson.EnumerateArray())
                        {
                            var experience = JsonSerializer.Deserialize<ExperienceResponse>(element.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                            if (element.TryGetProperty("Doctor", out JsonElement doctorJson) && doctorJson.ValueKind != JsonValueKind.Null)
                            {
                                experience.Doctor = JsonSerializer.Deserialize<DoctorResponse>(doctorJson.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                            }
                            else
                            {
                                experience.Doctor = new DoctorResponse { FullName = "Không có thông tin" };
                            }

                            if (element.TryGetProperty("Specialty", out JsonElement specialtyJson) && specialtyJson.ValueKind != JsonValueKind.Null)
                            {
                                experience.Specialty = JsonSerializer.Deserialize<SpecialtyResponse>(specialtyJson.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                            }

                            experiencesList.Add(experience);
                        }

                        Experiences = experiencesList;
                    }
                    else
                    {
                        Experiences = new List<ExperienceResponse>();
                    }

                    TotalItems = Experiences.Count;
                }
                else
                {
                    _logger.LogError($"Lỗi khi tải danh sách Experience: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tải dữ liệu Experience: {ex.Message}");
            }

            CurrentPage = page;
            ViewData["Title"] = $"Danh sách Experience của {DoctorFullName}";
            return Page();
        }
    }
}
