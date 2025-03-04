using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MediPlat.Model.Model;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
namespace MediPlat.RazorPage.Pages.Doctors
{
    [Authorize(Policy = "DoctorPolicy")]
    public class ProfileModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public ProfileModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public Doctor Doctor { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Request.Cookies["AuthToken"];

            if (string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError("", "Session expired, please login again!");
                return Page();
            }

            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

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
