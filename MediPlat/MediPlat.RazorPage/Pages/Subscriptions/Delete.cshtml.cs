using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MediPlat.Model.Model;

namespace MediPlat.RazorPage.Pages.Subscriptions
{
    public class DeleteModel : PageModel
    {
        private readonly MediPlat.Model.Model.MediPlatContext _context;

        public DeleteModel(MediPlat.Model.Model.MediPlatContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Subscription Subscription { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions.FirstOrDefaultAsync(m => m.Id == id);

            if (subscription == null)
            {
                return NotFound();
            }
            else
            {
                Subscription = subscription;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription != null)
            {
                Subscription = subscription;
                _context.Subscriptions.Remove(Subscription);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
