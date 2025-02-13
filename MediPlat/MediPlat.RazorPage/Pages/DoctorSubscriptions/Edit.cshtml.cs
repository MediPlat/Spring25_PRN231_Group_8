using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MediPlat.Model.Model;

namespace MediPlat.RazorPage.Pages.DoctorSubscriptions
{
    public class EditModel : PageModel
    {
        private readonly MediPlat.Model.Model.MediPlatContext _context;

        public EditModel(MediPlat.Model.Model.MediPlatContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DoctorSubscription DoctorSubscription { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorsubscription =  await _context.DoctorSubscriptions.FirstOrDefaultAsync(m => m.Id == id);
            if (doctorsubscription == null)
            {
                return NotFound();
            }
            DoctorSubscription = doctorsubscription;
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

            var existingSubscription = await _context.DoctorSubscriptions.FindAsync(DoctorSubscription.Id);
            if (existingSubscription != null)
            {
                existingSubscription.EnableSlot = DoctorSubscription.EnableSlot;
                await _context.SaveChangesAsync();
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorSubscriptionExists(DoctorSubscription.Id))
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

        private bool DoctorSubscriptionExists(Guid id)
        {
            return _context.DoctorSubscriptions.Any(e => e.Id == id);
        }
    }
}
