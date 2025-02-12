using MediPlat.Model.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MediPlat.RazorPage.Pages.PatientPages
{
    public class IndexModel : PageModel
    {
        private readonly MediPlatContext _context;

        public IndexModel(MediPlatContext context)
        {
            _context = context;
        }

        public IList<Patient> Patient { get; set; } = default!;

        public async Task OnGetAsync()
        {
            //Patient = await _context.Patients.ToListAsync();

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Key", "Value");

                using (HttpResponseMessage response = await httpClient.GetAsync("https://localhost:7002/odata/patient"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        Patient = JsonConvert.DeserializeObject<List<Patient>>(apiResponse);
                    }

                }
            }
        }
    }
}
