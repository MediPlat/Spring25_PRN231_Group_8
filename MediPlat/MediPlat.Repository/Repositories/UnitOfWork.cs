using System;
using System.Threading.Tasks;
using MediPlat.Model.Model;
using MediPlat.Repository.IRepositories;

namespace MediPlat.Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MediPlatContext _context;
        public IGenericRepository<DoctorSubscription> DoctorSubscriptions { get; }
        public IGenericRepository<Subscription> Subscriptions { get; }

        public UnitOfWork(MediPlatContext context)
        {
            _context = context;
            DoctorSubscriptions = new GenericRepository<DoctorSubscription>(context);
            Subscriptions = new GenericRepository<Subscription>(context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
