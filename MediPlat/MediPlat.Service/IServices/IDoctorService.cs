
using MediPlat.Model.Model;
using MediPlat.Model.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Service.IServices
{
    public interface IDoctorService
    {
        public Task<Doctor> GetByID(Guid id);
        public Task<bool> Banned(Guid id);
        public Doctor Update(DoctorSchema doctor, string id);
        public Task<bool> ChangePassword(ChangePassword change, Guid id);
        public Task<Doctor> Create(DoctorSchema doctor);
        IQueryable<Doctor> GetAllDoctor();
    }
}
