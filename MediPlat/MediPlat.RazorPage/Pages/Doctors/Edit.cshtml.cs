using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using System.Text;

namespace MediPlat.RazorPage.Pages.Doctors
{
    public class EditModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<IndexModel> _logger;

        public EditModel(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory, ILogger<IndexModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        [BindProperty]
        public DoctorRequest doctorRequest { get; set; } = default!;
        [BindProperty]
        public DoctorResponse doctorResponse { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }

            var client = _clientFactory.CreateClient("UntrustedClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("https://localhost:7002/odata/Doctors/profile");

            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                doctorResponse = JsonConvert.DeserializeObject<DoctorResponse>(apiResponse);

                if (doctorResponse != null)
                {
                    doctorRequest = new DoctorRequest
                    {
                        Id = doctorResponse.Id,
                        UserName = doctorResponse.UserName,
                        FullName = doctorResponse.FullName,
                        Email = doctorResponse.Email,
                        FeePerHour = doctorResponse.FeePerHour,
                        Degree = doctorResponse.Degree,
                        AcademicTitle = doctorResponse.AcademicTitle,
                        PhoneNumber = doctorResponse.PhoneNumber
                    };
                }
            }
            else
            {
                ModelState.AddModelError("", "Failed to retrieve doctor profile.");
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            
            if (string.IsNullOrEmpty(doctorRequest.Password))
            {
                ModelState.Remove("doctorRequest.Password");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }

            var client = _clientFactory.CreateClient("UntrustedClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jsonContent = JsonConvert.SerializeObject(doctorRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PatchAsync("https://localhost:7002/odata/Doctors/profile/update", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Update thành công!");
                return RedirectToPage("/Doctors/Profile");
            }
            else
            {
                ModelState.AddModelError("", "Failed to update profile.");
            }
            return Page();
        }
    }
}
