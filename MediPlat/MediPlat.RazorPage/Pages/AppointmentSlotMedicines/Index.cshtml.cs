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
    public class IndexModel : PageModel
    {
        private readonly MediPlatContext _context;

        public IndexModel(MediPlatContext context)
        {
            _context = context;
        }

        public IList<AppointmentSlotMedicine> AppointmentSlotMedicine { get;set; } = default!;

        public async Task OnGetAsync()
        {
            AppointmentSlotMedicine = await _context.AppointmentSlotMedicines
                .Include(a => a.AppointmentSlot)
                .Include(a => a.Medicine).ToListAsync();
        }
    }
}
