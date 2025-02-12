using MediPlat.Model.Model;
using MediPlat.Model.Schema;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IServices;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Service.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IGenericRepository<Doctor> _doctor_repository;
        public DoctorService(IGenericRepository<Doctor> doctor_repository)
        {
            _doctor_repository = doctor_repository;
        }

        public async Task<bool> Banned(Guid id)
        {
            var doctor = await _doctor_repository.GetIdAsync(id);
            doctor.Status = "Inactive";
            _doctor_repository.Update(doctor);
            return true;
        }

        public async Task<bool> ChangePassword(ChangePassword change, Guid id)
        {
            var doctor = await _doctor_repository.GetIdAsync(id);
            if (doctor.Password.Equals(change.Old_Password))
            {
                if (change.New_Password.Equals(change.Comfirm_Password))
                {
                    doctor.Password = change.New_Password;
                    _doctor_repository.Update(doctor);
                    return true;
                }
            }
            return false;

        }

        public async Task<Doctor> GetByID(Guid id)
        {
            var doctor = await _doctor_repository.GetIdAsync(id);
            return doctor;
        }

        public Doctor Update(DoctorSchema doctor, string id)
        {
            Doctor profile = _doctor_repository.GetId(Guid.Parse(id));
            if (!doctor.FullName.IsNullOrEmpty()) { profile.FullName = doctor.FullName; }
            if (!doctor.Email.IsNullOrEmpty()) { profile.Email = doctor.Email; }
            if (doctor.FeePerSession.HasValue) { profile.FeePerSession = doctor.FeePerSession; }
            if (!doctor.Degree.IsNullOrEmpty()) { profile.Degree = doctor.Degree; }
            if (!doctor.AcademicTitle.IsNullOrEmpty()) { profile.AcademicTitle = doctor.AcademicTitle; }
            if (!doctor.PhoneNumber.IsNullOrEmpty()) { profile.PhoneNumber = doctor.PhoneNumber; }

            profile.PhoneNumber = doctor.PhoneNumber;

            _doctor_repository.Update(profile);
            return profile;
        }
    }
}
