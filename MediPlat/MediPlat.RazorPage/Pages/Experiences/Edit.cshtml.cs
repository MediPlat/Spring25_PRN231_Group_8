using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MediPlat.Model.Model;

namespace MediPlat.RazorPage.Pages.Experiences
{
    public class EditModel : PageModel
    {
        private readonly MediPlat.Model.Model.MediPlatContext _context;

        public EditModel(MediPlat.Model.Model.MediPlatContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Experience Experience { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var experience =  await _context.Experiences.FirstOrDefaultAsync(m => m.Id == id);
            if (experience == null)
            {
                return NotFound();
            }
            Experience = experience;
           ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Id");
           ViewData["SpecialtyId"] = new SelectList(_context.Specialties, "Id", "Id");
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

            _context.Attach(Experience).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExperienceExists(Experience.Id))
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

        private bool ExperienceExists(Guid id)
        {
            return _context.Experiences.Any(e => e.Id == id);
        }
    }
}
