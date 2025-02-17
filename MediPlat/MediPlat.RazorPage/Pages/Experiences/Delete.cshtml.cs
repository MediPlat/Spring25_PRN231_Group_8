using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MediPlat.Model.Model;

namespace MediPlat.RazorPage.Pages.Experiences
{
    public class DeleteModel : PageModel
    {
        private readonly MediPlatContext _context;

        public DeleteModel(MediPlatContext context)
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

            var experience = await _context.Experiences.FirstOrDefaultAsync(m => m.Id == id);

            if (experience == null)
            {
                return NotFound();
            }
            else
            {
                Experience = experience;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var experience = await _context.Experiences.FindAsync(id);
            if (experience != null)
            {
                Experience = experience;
                _context.Experiences.Remove(Experience);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
