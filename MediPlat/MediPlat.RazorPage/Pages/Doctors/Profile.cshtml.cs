using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MediPlat.Model.Model;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;

namespace MediPlat.RazorPage.Pages.Doctors
{
    [Authorize(Policy = "DoctorPolicy")]
    public class ProfileModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProfileModel(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public Doctor Doctor { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // ✅ Sử dụng TokenHelper để lấy token
            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }

            var client = _clientFactory.CreateClient("UntrustedClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client.GetAsync("https://localhost:7002/odata/Doctors/profile");

            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                Doctor = JsonConvert.DeserializeObject<Doctor>(apiResponse);
            }
            else
            {
                ModelState.AddModelError("", "Failed to retrieve doctor profile.");
            }

            return Page();
        }
    }
}
