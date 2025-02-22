using AutoMapper;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject.Patient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace MediPlat.RazorPage.Pages.Patients
{
    public class ProfileModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public ProfileModel(IHttpContextAccessor httpContextAccessor, HttpClient httpClient, IMapper mapper)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        [BindProperty]
        public Patient Patient { get; set; } = new();
        public string? Message { get; set; }

        public async Task OnGetAsync()
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

            using (HttpResponseMessage response = await _httpClient.GetAsync($"https://localhost:7002/odata/patient/{HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value}"))
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

            using (HttpResponseMessage response = await _httpClient.PutAsJsonAsync<PatientRequest>($"https://localhost:7002/odata/patient/{HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value}", _mapper.Map<PatientRequest>(Patient)))
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
