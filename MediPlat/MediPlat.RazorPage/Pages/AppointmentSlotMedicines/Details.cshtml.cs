using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MediPlat.Model.Model;

namespace MediPlat.RazorPage.Pages.AppointmentSlotMedicines
{
    public class DetailsModel : PageModel
    {
        private readonly MediPlatContext _context;

        public DetailsModel(MediPlatContext context)
        {
            _context = context;
        }

        public AppointmentSlotMedicine AppointmentSlotMedicine { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointmentslotmedicine = await _context.AppointmentSlotMedicines.FirstOrDefaultAsync(m => m.AppointmentSlotMedicineId == id);
            if (appointmentslotmedicine == null)
            {
                return NotFound();
            }
            else
            {
                AppointmentSlotMedicine = appointmentslotmedicine;
            }
            return Page();
        }
    }
}
