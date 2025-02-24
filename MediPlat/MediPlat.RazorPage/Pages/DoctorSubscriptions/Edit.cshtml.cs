﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MediPlat.RazorPage.Pages.DoctorSubscriptions
{
    [Authorize(Policy = "DoctorPolicy")]
    public class EditModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly ILogger<EditModel> _logger;

        public EditModel(IHttpContextAccessor httpContextAccessor, HttpClient httpClient, ILogger<EditModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _logger = logger;
        }

        [BindProperty]
        public DoctorSubscription DoctorSubscription { get; set; } = default!;

        [BindProperty]
        public short AdditionalEnableSlot { get; set; }
        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }
            if (token.StartsWith("Bearer "))
            {
                token = token.Substring("Bearer ".Length);
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7002/odata/DoctorSubscriptions/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return Forbid();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                DoctorSubscription = JsonSerializer.Deserialize<DoctorSubscription>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (DoctorSubscription.SubscriptionId == null || DoctorSubscription.SubscriptionId == Guid.Empty)
                {
                    ModelState.AddModelError("", "Failed to load SubscriptionId.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching the subscription details.");
            }

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }
            if (token.StartsWith("Bearer "))
            {
                token = token.Substring("Bearer ".Length);
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                if (DoctorSubscription.SubscriptionId == null || DoctorSubscription.SubscriptionId == Guid.Empty)
                {
                    ModelState.AddModelError("", "SubscriptionId is required.");
                    return Page();
                }

                short newEnableSlot = (short)(DoctorSubscription.EnableSlot + AdditionalEnableSlot);

                if (newEnableSlot > 1000)
                {
                    ModelState.AddModelError("", "Total EnableSlot cannot exceed 1000.");
                    return Page();
                }

                var requestData = new
                {
                    SubscriptionId = DoctorSubscription.SubscriptionId,
                    EnableSlot = newEnableSlot,
                    UpdateDate = DateTime.Now,
                    Status = DoctorSubscription.Status
                };

                var jsonContent = JsonSerializer.Serialize(requestData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"https://localhost:7002/odata/DoctorSubscriptions/{DoctorSubscription.Id}", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Update failed: {errorResponse}");
                    return Page();
                }

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the subscription.");
                return Page();
            }
        }
    }
}
