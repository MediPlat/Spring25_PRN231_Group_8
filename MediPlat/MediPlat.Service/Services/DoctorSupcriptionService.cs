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
        private readonly IGenericRepository<DoctorSubcription> _repository;
        public DoctorSupcriptionService(IGenericRepository<DoctorSubcription> repository)
        {
            _repository = repository;
        }

        public async Task<List<DoctorSubcription>> GetDoctorSubcriptions(Guid id)
        {
            List<DoctorSubcription> doctorSubcriptions = new List<DoctorSubcription>();
            doctorSubcriptions = _repository.GetList(s => s.DoctorId == id, s => s.Subscription).ToList();
            return doctorSubcriptions;
        }
    }
}
