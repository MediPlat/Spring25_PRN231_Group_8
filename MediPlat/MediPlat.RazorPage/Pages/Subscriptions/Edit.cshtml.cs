using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MediPlat.Model.Model;
using Microsoft.AspNetCore.Http;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;

namespace MediPlat.RazorPage.Pages.Subscriptions
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
        public SubscriptionRequest subscriptionRequest { get; set; } = new SubscriptionRequest()!;
        [BindProperty]
        public SubscriptionResponse subscriptionResponse { get; set; } = new SubscriptionResponse();
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

            string apiUrl = $"https://localhost:7002/odata/Subscriptions/{id}";
            var response = await client.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                subscriptionResponse = JsonConvert.DeserializeObject<SubscriptionResponse>(apiResponse);

                if (subscriptionResponse != null)
                {
                    subscriptionRequest = new SubscriptionRequest
                    {
                        Name = subscriptionResponse.Name,
                        Description = subscriptionResponse.Description,
                        CreatedDate = subscriptionResponse.CreatedDate,
                        EnableSlot = subscriptionResponse.EnableSlot,
                        Price = subscriptionResponse.Price,
                        UpdateDate = DateTime.Now
                    };
                }
            }
            else
            {
                ModelState.AddModelError("", "Failed to retrieve Subscription.");
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var id = subscriptionResponse.Id;

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

            var jsonContent = JsonConvert.SerializeObject(subscriptionRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            string apiUrl = $"https://localhost:7002/odata/Subscriptions/{id}";
            var response = await client.PutAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Update thành công!");
                return RedirectToPage("/Subscriptions/Index");
            }
            else
            {
                ModelState.AddModelError("", "Failed to update profile.");
            }
            return Page();
        }

       
    }
}
