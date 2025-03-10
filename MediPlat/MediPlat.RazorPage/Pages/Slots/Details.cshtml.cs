using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MediPlat.Model.Model;
using System.Security.Claims;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Microsoft.IdentityModel.Tokens;
using MediPlat.Model.RequestObject;
using AutoMapper;

namespace MediPlat.RazorPage.Pages.Slots
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string? _apiBaseUrl;
        private readonly IMapper _mapper;
        private readonly MediPlatContext _mediPlatContext;
        public DetailsModel(IHttpContextAccessor httpContextAccessor, HttpClient httpClient, IConfiguration configuration, IMapper mapper, MediPlatContext mediPlatContext)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _apiBaseUrl = configuration["ApiBaseUrl"];
            _mapper = mapper;
            _mediPlatContext = mediPlatContext;
        }

        public Slot Slot { get; set; } = default!;
        public IList<Model.Model.Profile> Profiles { get; set; }
        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
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

            using (HttpResponseMessage response = await _httpClient.GetAsync($"{_apiBaseUrl}/odata/Slots/GetSlotById/{id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(apiResponse))
                    {
                        Slot = JsonConvert.DeserializeObject<Slot>(apiResponse);
                    }
                }

            }

            using (HttpResponseMessage response = await _httpClient.GetAsync($"{_apiBaseUrl}/odata/Profiles?$filter=patientId eq {HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(apiResponse); // Parse JSON response
                    var profilesArray = jsonObject["value"]?.ToString(); // Extract "value" array

                    Profiles = JsonConvert.DeserializeObject<List<Model.Model.Profile>>(profilesArray);
                }

            }

            if (Slot == null || Profiles.IsNullOrEmpty())
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id, Guid profileId)
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

            using (HttpResponseMessage response = await _httpClient.GetAsync($"{_apiBaseUrl}/odata/Slots/GetSlotById/{id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(apiResponse))
                    {
                        Slot = JsonConvert.DeserializeObject<Slot>(apiResponse);
                    }
                }

            }
            if (Slot == null)
            {
                return NotFound();
            }

            // Fetch the patient and their profile
            var patientId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var patient = new Patient();
            using (HttpResponseMessage response = await _httpClient.GetAsync($"{_apiBaseUrl}/odata/Patients/{patientId}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //var jsonObject = JObject.Parse(apiResponse); // Parse JSON response
                    //var patientsArray = jsonObject["value"]?.ToString(); // Extract "value" array

                    patient = JsonConvert.DeserializeObject<Patient>(apiResponse);
                }
            }
            if (patient == null)
            {
                return NotFound();
            }

            // Check if the patient has enough balance
            if (patient.Balance < Slot.SessionFee)
            {
                ModelState.AddModelError("", "Insufficient balance.");
                return Page();
            }

            // Create AppointmentSlot
            var appointmentSlot = new AppointmentSlotRequest()
            {
                SlotId = Slot.Id,
                ProfileId = profileId,
                Status = "Pending",
                CreatedDate = DateTime.Now
            };

            using (HttpResponseMessage response = await _httpClient.PostAsJsonAsync<AppointmentSlotRequest>($"{_apiBaseUrl}/api/AppointmentSlot/CreateAppointmentSlot", appointmentSlot))
            {
                if (!response.IsSuccessStatusCode)
                {
                    RedirectToPage("Error");
                }
            }
            var createdAppointmentSlot = new List<AppointmentSlot>();
            using (HttpResponseMessage response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/AppointmentSlot/GetAppointmentSlotBySlotID/{Slot.Id}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(apiResponse))
                    {
                        createdAppointmentSlot = JsonConvert.DeserializeObject<List<AppointmentSlot>>(apiResponse);
                    }
                }

            }
            if (createdAppointmentSlot is null)
            {
                return NotFound();
            }
            // Create Transaction
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                DoctorId = (Guid)Slot.DoctorId,  // assuming the slot has a doctor
                AppointmentSlotId = createdAppointmentSlot.FirstOrDefault().Id,
                TransactionInfo = "Booking payment",
                Amount = (decimal)Slot.SessionFee,
                CreatedDate = DateTime.Now,
                Status = "Completed"
            };
            //using (HttpResponseMessage response = await _httpClient.PostAsJsonAsync<AppointmentSlotRequest>($"{_apiBaseUrl}/api/AppointmentSlot/CreateAppointmentSlot", appointmentSlot))
            //{
            //    if (response.IsSuccessStatusCode)
            //    {
            //        RedirectToPage("Index");
            //    }
            //    else
            //    {
            //        RedirectToPage("Error");
            //    }
            //}

            _mediPlatContext.Transactions.Add(transaction);
            await _mediPlatContext.SaveChangesAsync();

            // Decrease patient balance
            patient.Balance -= Slot.SessionFee;
            _mediPlatContext.Patients.Update(patient);
            await _mediPlatContext.SaveChangesAsync();

            // Redirect after successful booking
            return RedirectToPage("Index");
        }
    }
}
