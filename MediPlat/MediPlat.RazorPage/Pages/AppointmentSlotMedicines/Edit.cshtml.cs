using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MediPlat.Model.Model;

namespace MediPlat.RazorPage.Pages.AppointmentSlotMedicines
{
    public class EditModel : PageModel
    {
        private readonly MediPlatContext _context;

        public EditModel(MediPlatContext context)
        {
            _context = context;
        }

        [BindProperty]
        public AppointmentSlotMedicine AppointmentSlotMedicine { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointmentslotmedicine =  await _context.AppointmentSlotMedicines.FirstOrDefaultAsync(m => m.AppointmentSlotMedicineId == id);
            if (appointmentslotmedicine == null)
            {
                return NotFound();
            }
            AppointmentSlotMedicine = appointmentslotmedicine;
           ViewData["AppointmentSlotId"] = new SelectList(_context.AppointmentSlots, "Id", "Status");
           ViewData["MedicineId"] = new SelectList(_context.Medicines, "Id", "Name");
           ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Id");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(AppointmentSlotMedicine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentSlotMedicineExists(AppointmentSlotMedicine.AppointmentSlotMedicineId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool AppointmentSlotMedicineExists(Guid id)
        {
            return _context.AppointmentSlotMedicines.Any(e => e.AppointmentSlotMedicineId == id);
        }
    }
}
