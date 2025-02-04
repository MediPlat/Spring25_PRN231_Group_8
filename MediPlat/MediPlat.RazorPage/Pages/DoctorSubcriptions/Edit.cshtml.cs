using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MediPlat.Model;

namespace MediPlat.RazorPage.Pages_DoctorSubcriptions
{
    public class EditModel : PageModel
    {
        private readonly MediPlat.Model.MediPlatContext _context;

        public EditModel(MediPlat.Model.MediPlatContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DoctorSubscription DoctorSubcription { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorsubcription =  await _context.DoctorSubcriptions.FirstOrDefaultAsync(m => m.Id == id);
            if (doctorsubcription == null)
            {
                return NotFound();
            }
            DoctorSubcription = doctorsubcription;
           ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Id");
           ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "Id", "Id");
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

            _context.Attach(DoctorSubcription).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorSubcriptionExists(DoctorSubcription.Id))
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

        private bool DoctorSubcriptionExists(Guid id)
        {
            return _context.DoctorSubcriptions.Any(e => e.Id == id);
        }
    }
}
