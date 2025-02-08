using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MediPlat.Model.Model;
namespace MediPlat.RazorPage.Pages.DoctorSubscription
{
    public class IndexModel : PageModel
    {
        private readonly MediPlat.Model.Model.MediPlatContext _context;
        public IndexModel(MediPlat.Model.Model.MediPlatContext context)
        {
            _context = context;
        }
        public IList<MediPlat.Model.Model.DoctorSubscription> DoctorSubscription { get; set; } = default!;
        public async Task OnGetAsync()
        {
            DoctorSubscription = await _context.DoctorSubscriptions
                .Include(d => d.Doctor)
                .Include(d => d.Subscription).ToListAsync();
        }
    }
}