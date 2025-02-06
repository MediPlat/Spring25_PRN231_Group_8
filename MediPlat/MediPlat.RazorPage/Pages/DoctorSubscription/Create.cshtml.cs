using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MediPlat.Model.Model;

namespace MediPlat.RazorPage.Pages.DoctorSubscription
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
        ViewData["SubscriptionId"] = new SelectList(_context.Subscriptions, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public Model.Model.DoctorSubscription DoctorSubscription { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.DoctorSubscriptions.Add(DoctorSubscription);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
