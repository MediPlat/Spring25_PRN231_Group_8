﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MediPlat.Model.Model;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.Numerics;
using MediPlat.Model.ResponseObject;
using System.Security.Claims;

namespace MediPlat.RazorPage.Pages.Doctors
{
    [Authorize(Policy = "DoctorPolicy")]
    public class ProfileModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<IndexModel> _logger;

        public ProfileModel(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory, ILogger<IndexModel> logger)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [BindProperty]
        public DoctorResponse doctor { get; set; }
        public bool IsAdmin { get; private set; } = false;
        public bool IsDoctor { get; private set; } = false;

        public async Task<IActionResult> OnGetAsync()
        {
            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }

            var client = _clientFactory.CreateClient("UntrustedClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            IsAdmin = userRole == "Admin";
            IsDoctor = userRole == "Doctor";
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var response = await client.GetAsync("https://localhost:7002/odata/Doctors/profile");

            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                doctor = JsonConvert.DeserializeObject<DoctorResponse>(apiResponse);
                if (doctor != null)
                {
                    var doctorFullName = doctor.FullName;
                    _logger.LogInformation($"Lấy thành công thông tin bác sĩ: {doctorFullName}");
                }
            }
            else
            {
                ModelState.AddModelError("", "Failed to retrieve doctorResponse profile.");
            }

            return Page();
        }
    }
}
