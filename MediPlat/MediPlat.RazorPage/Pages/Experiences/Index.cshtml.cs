using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MediPlat.Model.Model;
using Newtonsoft.Json;

namespace MediPlat.RazorPage.Pages.Experiences
{
    public class IndexModel : PageModel
    {
        private readonly MediPlatContext _context;

        public IndexModel(MediPlatContext context)
        {
            _context = context;
        }

        public IList<Experience> Experience { get;set; } = default!;

        public async Task OnGetAsync()
        {
            //Patient = await _context.Patients.ToListAsync();

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Key", "Value");

                using (HttpResponseMessage response = await httpClient.GetAsync("https://localhost:7002/odata/experience"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        Experience = JsonConvert.DeserializeObject<List<Experience>>(apiResponse);
                    }

                }
            }
        }
    }
}
