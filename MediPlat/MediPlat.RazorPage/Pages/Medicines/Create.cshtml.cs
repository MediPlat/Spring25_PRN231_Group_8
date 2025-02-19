using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MediPlat.Model.Model;
using Microsoft.AspNetCore.Authorization;

namespace MediPlat.RazorPage.Pages.Medicines
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
            return Page();
        }

        [BindProperty]
        public Medicine Medicine { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Medicines.Add(Medicine);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
