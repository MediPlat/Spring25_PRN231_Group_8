using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MediPlat.Model.Model;
using Microsoft.AspNetCore.Authorization;

namespace MediPlat.RazorPage.Pages.Experiences
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
        ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Id");
        ViewData["SpecialtyId"] = new SelectList(_context.Specialties, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public Experience Experience { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Experiences.Add(Experience);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
