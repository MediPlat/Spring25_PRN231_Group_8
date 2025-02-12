using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MediPlat.Model.Model;

namespace MediPlat.RazorPage.Pages.Experiences
{
    public class IndexModel : PageModel
    {
        private readonly MediPlat.Model.Model.MediPlatContext _context;

        public IndexModel(MediPlat.Model.Model.MediPlatContext context)
        {
            _context = context;
        }

        public IList<Experience> Experience { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Experience = await _context.Experiences
                .Include(e => e.Doctor)
                .Include(e => e.Specialty).ToListAsync();
        }
    }
}
