using MediPlat.Model.Model;
using MediPlat.Repository.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace MediPlat.Repository.Repositories
{
    public class SubscriptionRepository : GenericRepository<Subscription>, ISubscriptionRepository
    {
        private readonly MediPlatContext _context;

        public SubscriptionRepository(MediPlatContext context) : base(context)
        {
            _context = context;
        }
    }
}
