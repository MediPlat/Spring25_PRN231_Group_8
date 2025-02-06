using MediPlat.Model.Model;
using MediPlat.Repository.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace MediPlat.Repository.Repositories
{
    public class DoctorSubscriptionRepository : GenericRepository<DoctorSubscription>, IDoctorSubscriptionRepository
    {
        private readonly MediPlatContext _context;

        public DoctorSubscriptionRepository(MediPlatContext context) : base(context)
        {
            _context = context;
        }
    }
}
