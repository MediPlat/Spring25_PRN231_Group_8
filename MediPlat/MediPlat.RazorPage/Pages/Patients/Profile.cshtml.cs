using AutoMapper;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject.Patient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace MediPlat.RazorPage.Pages.Patients
{
    public class ProfileModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string? _apiBaseUrl;

        public ProfileModel(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IMapper mapper, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _apiBaseUrl = configuration["ApiBaseUrl"]; 
        }

        [BindProperty]
        public Patient Patient { get; set; } = new();
        public string? Message { get; set; }
        public string? JWTToken { get { return _httpClient.DefaultRequestHeaders.Authorization?.Parameter; } }
        public async Task OnGetAsync()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (token.IsNullOrEmpty())
            {
                Console.WriteLine("⚠️ Không tìm thấy token, chuyển hướng đến trang login...");
                RedirectToPage("/Auth/Login");
            }
            if (token.StartsWith("Bearer "))
            {
                token = token.Substring("Bearer ".Length);
            }

            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using (HttpResponseMessage response = await _httpClient.GetAsync($"{_apiBaseUrl}/odata/Patients/{HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    Patient = JsonConvert.DeserializeObject<Patient>(apiResponse);
                }
            }
        }

        public async Task OnPostAsync()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("⚠️ Không tìm thấy token ở Index.cshtml.cs của DoctorSubscription, chuyển hướng đến trang login...");
                RedirectToPage("/Auth/Login");
            }
            if (token.StartsWith("Bearer "))
            {
                token = token.Substring("Bearer ".Length);
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using (HttpResponseMessage response = await _httpClient.PutAsJsonAsync<PatientRequest>($"{_apiBaseUrl}/odata/Patients/{HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value}", _mapper.Map<PatientRequest>(Patient)))
            {
                if (response.IsSuccessStatusCode)
                {
                    Message = "Update profile successful!";
                }
                else
                {
                    Message = "Update failed!";
                }
            }
        }
    }
}
