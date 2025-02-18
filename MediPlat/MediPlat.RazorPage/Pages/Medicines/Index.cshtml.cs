using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MediPlat.Model.Model;
using Microsoft.AspNetCore.Authorization;

namespace MediPlat.RazorPage.Pages.Medicines
{
    public class IndexModel : PageModel
    {
        private readonly MediPlatContext _context;

        public IndexModel(MediPlatContext context)
        {
            _context = context;
        }

        public IList<Medicine> Medicine { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Medicine = await _context.Medicines.ToListAsync();
        }
    }
}
