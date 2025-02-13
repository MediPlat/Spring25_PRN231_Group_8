using MediPlat.Model.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MediPlat.RazorPage.Pages
{
    public class IndexModel : PageModel
    {
        private readonly MediPlat.Model.Model.MediPlatContext _context;

        public IndexModel(MediPlat.Model.Model.MediPlatContext context)
        {
            _context = context;
        }

        public IList<DoctorSubscription> DoctorSubscription { get; set; } = default!;

        public async Task OnGetAsync()
        {
            DoctorSubscription = await _context.DoctorSubscriptions
                .Include(d => d.Doctor)
                .Include(d => d.Subscription)
                .ToListAsync();
        }
    }

}
