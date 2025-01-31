using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MediPlat.Model;

namespace MediPlat.RazorPage.Pages_DoctorSubcriptions
{
    public class IndexModel : PageModel
    {
        private readonly MediPlat.Model.MediPlatContext _context;

        public IndexModel(MediPlat.Model.MediPlatContext context)
        {
            _context = context;
        }

        public IList<DoctorSubscription> DoctorSubcription { get;set; } = default!;

        public async Task OnGetAsync()
        {
            DoctorSubcription = await _context.DoctorSubcriptions
                .Include(d => d.Doctor)
                .Include(d => d.Subscription).ToListAsync();
        }
    }
}
