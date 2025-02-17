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
    public class DetailsModel : PageModel
    {
        private readonly MediPlatContext _context;

        public DetailsModel(MediPlatContext context)
        {
            _context = context;
        }

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
    }
}
