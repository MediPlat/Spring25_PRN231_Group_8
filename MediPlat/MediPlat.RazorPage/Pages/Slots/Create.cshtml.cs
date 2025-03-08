using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MediPlat.Model.Model;

namespace MediPlat.RazorPage.Pages.Slots
{
    public class CreateModel : PageModel
    {
        private readonly MediPlat.Model.Model.MediPlatContext _context;

        public CreateModel(MediPlat.Model.Model.MediPlatContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Id");
        ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public Slot Slot { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Slots.Add(Slot);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
