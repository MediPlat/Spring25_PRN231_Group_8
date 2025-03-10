﻿using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using static MediPlat.RazorPage.Pages.Experiences.IndexModel;

namespace MediPlat.RazorPage.Pages.Prescriptions
{
    [Authorize(Policy = "DoctorPolicy")]
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor, ILogger<CreateModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        [BindProperty]
        public AppointmentSlotRequest AppointmentSlot { get; set; } = new AppointmentSlotRequest();

        public List<SlotResponse> Slots { get; set; } = new List<SlotResponse>();
        public List<ProfileResponse> Profiles { get; set; } = new List<ProfileResponse>();
        public List<MedicineResponse> Medicines { get; set; } = new List<MedicineResponse>();

        public async Task<IActionResult> OnGetAsync()
        {
            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }

            var client = _clientFactory.CreateClient("UntrustedClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            Slots = await FetchData<SlotResponse>(client, "https://localhost:7002/odata/Slots", isOData: false);
            Profiles = await FetchData<ProfileResponse>(client, "https://localhost:7002/odata/Profiles", isOData: true);
            Medicines = await FetchData<MedicineResponse>(client, "https://localhost:7002/odata/Medicines", isOData: true);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation($"Received {AppointmentSlot.Medicines.Count} medicines.");
            var token = TokenHelper.GetCleanToken(_httpContextAccessor.HttpContext);
            var client = _clientFactory.CreateClient("UntrustedClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var jsonContent = new StringContent(JsonSerializer.Serialize(AppointmentSlot), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7002/odata/AppointmentSlots", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                return Page();
            }

            return RedirectToPage("/Prescriptions/Index");
        }

        private async Task<List<T>> FetchData<T>(HttpClient client, string url, bool isOData = true)
        {
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new List<T>();
            }

            var jsonBytes = await response.Content.ReadAsByteArrayAsync();
            var json = Encoding.UTF8.GetString(jsonBytes);

            try
            {
                if (isOData)
                {
                    return JsonSerializer.Deserialize<ODataResponse<T>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })?.Value ?? new List<T>();
                }
                else
                {
                    return JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<T>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ JSON Deserialize Error: {ex.Message}");
                return new List<T>();
            }
        }
    }
}
