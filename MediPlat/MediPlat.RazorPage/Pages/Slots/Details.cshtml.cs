using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MediPlat.Model.Model;

namespace MediPlat.RazorPage.Pages.Slots
{
    public class DetailsModel : PageModel
    {
        private readonly MediPlat.Model.Model.MediPlatContext _context;

        public DetailsModel(MediPlat.Model.Model.MediPlatContext context)
        {
            _context = context;
        }

        public Slot Slot { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slot = await _context.Slots.FirstOrDefaultAsync(m => m.Id == id);
            if (slot == null)
            {
                return NotFound();
            }
            else
            {
                Slot = slot;
            }
            return Page();
        }
    }
}
