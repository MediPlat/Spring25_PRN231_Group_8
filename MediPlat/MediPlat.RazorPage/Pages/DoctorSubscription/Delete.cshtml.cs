﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MediPlat.Model.Model;

namespace MediPlat.RazorPage.Pages.DoctorSubscription
{
    public class DeleteModel : PageModel
    {
        private readonly MediPlatContext _context;

        public DeleteModel(MediPlatContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Model.Model.DoctorSubscription DoctorSubscription { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorsubscription = await _context.DoctorSubscriptions.FirstOrDefaultAsync(m => m.Id == id);

            if (doctorsubscription == null)
            {
                return NotFound();
            }
            else
            {
                DoctorSubscription = doctorsubscription;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorsubscription = await _context.DoctorSubscriptions.FindAsync(id);
            if (doctorsubscription != null)
            {
                DoctorSubscription = doctorsubscription;
                _context.DoctorSubscriptions.Remove(DoctorSubscription);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
