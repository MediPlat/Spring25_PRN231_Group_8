using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MediPlat.Model.Model;
using System.Security.Claims;

namespace MediPlat.RazorPage.Pages.DoctorSubscriptions
{
    public class DetailsModel : PageModel
    {
        private readonly MediPlatContext _context;

        public DetailsModel(MediPlatContext context)
        {
            _context = context;
        }

        public DoctorSubscription DoctorSubscription { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var doctorsubscription = await _context.DoctorSubscriptions
                .FirstOrDefaultAsync(m => m.Id == id && m.DoctorId == doctorId);

            if (doctorsubscription == null)
            {
                return Forbid();
            }

            DoctorSubscription = doctorsubscription;
            return Page();
        }
    }
}
