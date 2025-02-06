using MediPlat.Model;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Service.Services
{
    public class DoctorSupcriptionService : IDoctorSupcriptionService
    {
        private readonly IGenericRepository<DoctorSubscription> _repository;
        public DoctorSupcriptionService(IGenericRepository<DoctorSubscription> repository)
        {
            _repository = repository;
        }

        public async Task<List<DoctorSubscription>> GetDoctorSubcriptions(Guid id)
        {
            List<DoctorSubscription> doctorSubscriptions = new List<DoctorSubscription>();
            doctorSubscriptions =  _repository.GetList(s => s.DoctorId == id, s => s.Subscription!).ToList();
            return doctorSubscriptions;
        }


    }
}
