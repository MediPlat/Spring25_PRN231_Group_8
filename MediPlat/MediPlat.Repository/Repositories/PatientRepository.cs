using MediPlat.Model;
using MediPlat.Repository.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Repository.Repositories
{
    public class PatientRepository : GenericRepository<Patient>, IPatientRepository
    {
        private readonly MediPlatContext _context;

        public PatientRepository(MediPlatContext context) : base(context)
        {
            _context = context;
        }
    }
}
