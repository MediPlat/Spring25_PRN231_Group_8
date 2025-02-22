using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MediPlat.Model.Model;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace MediPlat.RazorPage.Pages.Experiences
{
    public class EditModel : PageModel
    {
        private readonly MediPlatContext _context;

        public EditModel(MediPlatContext context)
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

            var doctorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var existingExperience = await _context.Experiences.FirstOrDefaultAsync(e => e.Id == Experience.Id && e.DoctorId == doctorId);

            if (existingExperience == null)
            {
                return Forbid();
            }

            existingExperience.Title = Experience.Title;
            existingExperience.Description = Experience.Description;
            existingExperience.Certificate = Experience.Certificate;
            existingExperience.SpecialtyId = Experience.SpecialtyId;

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }


        private bool ExperienceExists(Guid id)
        {
            return _context.Experiences.Any(e => e.Id == id);
        }
    }
}
