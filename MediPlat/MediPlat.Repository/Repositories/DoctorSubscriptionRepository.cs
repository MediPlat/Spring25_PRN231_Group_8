using MediPlat.Model;
using MediPlat.Repository.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Repository.Repositories
{
    public class DoctorSubscriptionRepository : GenericRepository<DoctorSubscription>, IDoctorSubscriptionRepository
    {
        private readonly MediPlatContext _context;

        public DoctorSubscriptionRepository() : base(new MediPlatContext())
        {
            _context = new MediPlatContext();
        }

        public async Task<IEnumerable<DoctorSubscription>> GetAllDoctorSubscriptionsAsync()
        {
            return await _context.DoctorSubscriptions.ToListAsync();
        }

        public async Task<DoctorSubscription> GetDoctorSubscriptionByIdAsync(Guid id)
        {
            return await _context.DoctorSubscriptions.FindAsync(id);
        }

        public async Task AddDoctorSubscriptionAsync(DoctorSubscription doctorSubscription)
        {
            _context.DoctorSubscriptions.Add(doctorSubscription);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDoctorSubscriptionAsync(DoctorSubscription doctorSubscription)
        {
            _context.Entry(doctorSubscription).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDoctorSubscriptionAsync(Guid id)
        {
            var doctorSubscription = await _context.DoctorSubscriptions.FindAsync(id);
            if (doctorSubscription != null)
            {
                _context.DoctorSubscriptions.Remove(doctorSubscription);
                await _context.SaveChangesAsync();
            }
        }
    }
}
