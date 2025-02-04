using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MediPlat.Model;

namespace MediPlat.RazorPage.Pages_DoctorSubcriptions
{
    public class CreateModel : PageModel
    {
        private readonly MediPlat.Model.MediPlatContext _context;

        public CreateModel(MediPlat.Model.MediPlatContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Id");
        ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public DoctorSubscription DoctorSubcription { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.DoctorSubcriptions.Add(DoctorSubcription);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
