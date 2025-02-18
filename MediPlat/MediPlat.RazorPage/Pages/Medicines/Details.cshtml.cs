using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MediPlat.Model.Model;
using Microsoft.AspNetCore.Authorization;

namespace MediPlat.RazorPage.Pages.Medicines
{
    public class DetailsModel : PageModel
    {
        private readonly MediPlatContext _context;

        public DetailsModel(MediPlatContext context)
        {
            _context = context;
        }

        public Medicine Medicine { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicine = await _context.Medicines.FirstOrDefaultAsync(m => m.Id == id);
            if (medicine == null)
            {
                return NotFound();
            }
            else
            {
                Medicine = medicine;
            }
            return Page();
        }
    }
}
