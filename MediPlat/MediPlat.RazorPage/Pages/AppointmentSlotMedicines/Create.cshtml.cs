using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MediPlat.Model.Model;

namespace MediPlat.RazorPage.Pages.AppointmentSlotMedicines
{
    public class CreateModel : PageModel
    {
        private readonly MediPlatContext _context;

        public CreateModel(MediPlatContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["AppointmentSlotId"] = new SelectList(_context.AppointmentSlots, "Id", "Status");
        ViewData["MedicineId"] = new SelectList(_context.Medicines, "Id", "Name");
        ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public AppointmentSlotMedicine AppointmentSlotMedicine { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.AppointmentSlotMedicines.Add(AppointmentSlotMedicine);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
