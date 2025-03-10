using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MediPlat.Model.Model;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MediPlat.RazorPage.Pages.Slots
{
    public class IndexModel : PageModel
    {

        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string? _apiBaseUrl;

        public IndexModel(IHttpContextAccessor httpContextAccessor, HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _apiBaseUrl = configuration["ApiBaseUrl"];
        }

        public IList<Slot> Slot { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("⚠️ Không tìm thấy token ở Index.cshtml.cs của Slot, chuyển hướng đến trang login...");
                RedirectToPage("/Auth/Login");
            }
            if (token.StartsWith("Bearer "))
            {
                token = token.Substring("Bearer ".Length);
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            //https://localhost:7002/odata/Slots?$expand=Doctor,Service($expand=Specialty) (use expand to include property in response obj)
            using (HttpResponseMessage response = await _httpClient.GetAsync($"{_apiBaseUrl}/odata/Slots?$expand=Doctor,Service($expand=Specialty)"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(apiResponse); // Parse JSON response
                    var slotsArray = jsonObject["value"]?.ToString(); // Extract "value" array

                    if (!string.IsNullOrEmpty(slotsArray))
                    {
                        Slot = JsonConvert.DeserializeObject<List<Slot>>(slotsArray);
                    }
                }
            }
        }
    }
}
