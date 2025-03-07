using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MediPlat.Model.Model;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace MediPlat.RazorPage.Pages.Doctors
{
    [Authorize(Roles = "Doctor, Admin")]
    public class DoctorModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DoctorModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IList<Doctor> DoctorList { get; set; } = new List<Doctor>();

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Request.Cookies["AuthToken"];

            if (string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError("", "Session expired, please login again!");
                return Page();
            }

            var client = _httpClientFactory.CreateClient("UntrustedClient");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("https://localhost:7002/odata/Doctors/all_doctor");

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                DoctorList = JsonConvert.DeserializeObject<List<Doctor>>(apiResponse);
            }
            return Page();
        }


        public async Task<IActionResult> OnPostToggleStatusAsync(Guid id, string status)
        {
            try
            {
                var newStatus = status == "Active" ? "Banned" : "Active";
                var payload = new { Status = newStatus };
                var content = new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json");
                var client = _httpClientFactory.CreateClient("UntrustedClient");
                var response = await client.PatchAsync($"https://localhost:7002/odata/Doctors/all_doctor", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to update doctor status.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            }
            return Page();
        }
    }
}
