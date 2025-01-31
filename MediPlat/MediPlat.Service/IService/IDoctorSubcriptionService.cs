using MediPlat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Service.IService
{
    public interface IDoctorSubscriptionService
    {
        Task<IEnumerable<DoctorSubscription>> GetAllDoctorSubscriptionsAsync();
        Task<DoctorSubscription> GetDoctorSubscriptionByIdAsync(Guid id);
        Task AddDoctorSubscriptionAsync(DoctorSubscription doctorSubscription);
        Task UpdateDoctorSubscriptionAsync(DoctorSubscription doctorSubscription);
        Task DeleteDoctorSubscriptionAsync(Guid id);
    }
}
