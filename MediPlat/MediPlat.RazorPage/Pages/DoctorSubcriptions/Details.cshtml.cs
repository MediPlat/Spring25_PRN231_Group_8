using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MediPlat.Model;

namespace MediPlat.RazorPage.Pages_DoctorSubcriptions
{
    public class DetailsModel : PageModel
    {
        private readonly MediPlat.Model.MediPlatContext _context;

        public DetailsModel(MediPlat.Model.MediPlatContext context)
        {
            _context = context;
        }

        public DoctorSubscription DoctorSubcription { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorsubcription = await _context.DoctorSubcriptions.FirstOrDefaultAsync(m => m.Id == id);
            if (doctorsubcription == null)
            {
                return NotFound();
            }
            else
            {
                DoctorSubcription = doctorsubcription;
            }
            return Page();
        }
    }
}
