using MediPlat.Model;
using MediPlat.Model.Model;
using MediPlat.Model.Schema;
using MediPlat.Repository.IRepositories;
using MediPlat.Repository.Repositories;
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
        private readonly IUnitOfWork _unitOfWork;
        public DoctorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Banned(Guid id)
        {
            var doctor = await _unitOfWork.Doctors.GetIdAsync(id);
            doctor.Status = "Inactive";
            _unitOfWork.Doctors.Update(doctor);
            return true;
        }

        public async Task<bool> ChangePassword(ChangePassword change, Guid id)
        {
            var doctor = await _unitOfWork.Doctors.GetIdAsync(id);
            if (doctor.Password.Equals(change.Old_Password))
            {
                if (change.New_Password.Equals(change.Comfirm_Password))
                {
                    doctor.Password = change.New_Password;
                    _unitOfWork.Doctors.Update(doctor);
                    return true;
                }
            }
            return false;

        }

        public async Task<Doctor> Create(DoctorSchema doctor)
        {
            Doctor d = new Doctor();
            Guid pass = Guid.NewGuid();
            d.Id = Guid.NewGuid();
            d.FullName = doctor.FullName;
            d.UserName = doctor.UserName;
            d.Email = doctor.Email;
            d.FeePerHour = doctor.FeePerHour;
            d.Degree = doctor.Degree;
            d.AcademicTitle = doctor.AcademicTitle;
            d.PhoneNumber = doctor.PhoneNumber;
            d.Balance = 0;
            d.Password = pass.ToString();
            d.JoinDate = DateTime.Now;
            d.AvatarUrl = null;
            d.Status = "Active";
            _unitOfWork.Doctors.Add(d);
            return d;
        }
        public IQueryable<Doctor> GetAllDoctor()
        {
            return _unitOfWork.Doctors.GetAll(d => d.DoctorSubscriptions);
        }

        public async Task<Doctor> GetByID(Guid id)
        {
            var doctor = await _unitOfWork.Doctors.GetIdAsync(id);
            return doctor;
        }

        public Doctor Update(DoctorSchema doctor, string id)
        {
            Doctor profile = _unitOfWork.Doctors.GetId(Guid.Parse(id));
            if (!doctor.FullName.IsNullOrEmpty()) { profile.FullName = doctor.FullName; }
            if (!doctor.Email.IsNullOrEmpty()) { profile.Email = doctor.Email; }
            if (!doctor.UserName.IsNullOrEmpty()) { profile.UserName = doctor.UserName; }
            if (doctor.FeePerHour.HasValue) { profile.FeePerHour = doctor.FeePerHour; }
            if (!doctor.Degree.IsNullOrEmpty()) { profile.Degree = doctor.Degree; }
            if (!doctor.AcademicTitle.IsNullOrEmpty()) { profile.AcademicTitle = doctor.AcademicTitle; }
            if (!doctor.PhoneNumber.IsNullOrEmpty()) { profile.PhoneNumber = doctor.PhoneNumber; }

            profile.PhoneNumber = doctor.PhoneNumber;

            _unitOfWork.Doctors.Update(profile);
            return profile;
        }
    }
}