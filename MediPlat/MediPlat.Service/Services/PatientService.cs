using AutoMapper;
using MediPlat.Model;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject.Auth;
using MediPlat.Model.RequestObject.Patient;
using MediPlat.Model.ResponseObject.Patient;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IServices;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Service.Services
{
    public class PatientService : IPatientService
    {
        static Guid guid;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PatientService(IUnitOfWork unitOfWork, IMapper mapper) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PatientResponse?> ChangePassword(ClaimsPrincipal claims, ChangePasswordRequest changePasswordRequest)
        {
            var id = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var patientId = new Guid(id);
            var patient = await _unitOfWork.Patients.GetAsync(p => p.Id == patientId && p.Status.Equals("Active"));

            if (patient == null)
            {
                throw new NotFoundException("Incorrect jwt token or patient deleted");
            }
            if (!changePasswordRequest.oldPassword.Equals(patient.Password))
            {
                throw new ArgumentException("Old password is incorrected.");
            }
            if (!changePasswordRequest.newPassword.Equals(changePasswordRequest.confirmNewPassword))
            {
                throw new ArgumentException("Confirm password is different from new password.");
            }
            patient.Password = changePasswordRequest.newPassword;
            _unitOfWork.Patients.Update(patient);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PatientResponse>(patient);
        }

        public async Task<PatientResponse?> Create(PatientRequest patientModel, ClaimsPrincipal claims)
        {
            Guid guid = Guid.NewGuid();
            _unitOfWork.Patients.Add(new Patient
            {
                Id = guid,
                UserName = patientModel.UserName,
                FullName = patientModel.FullName,
                Email = patientModel.Email,
                PhoneNumber = patientModel.PhoneNumber,
                Balance = patientModel.Balance,
                Address = patientModel.Address,
                Sex = patientModel.Sex,
                Status = patientModel.Status,
                JoinDate = patientModel.JoinDate,
                Password = patientModel.Password,
            });
            await _unitOfWork.SaveChangesAsync();
            return new PatientResponse
            {
                Id = guid,
                UserName = patientModel.UserName,
                FullName = patientModel.FullName,
                Email = patientModel.Email,
                PhoneNumber = patientModel.PhoneNumber,
                Balance = patientModel.Balance,
                JoinDate = patientModel.JoinDate,
                Sex = patientModel.Sex,
                Address = patientModel.Address,
                Status = patientModel.Status
            };
        }

        public async Task<PatientResponse?> DeleteById(ClaimsPrincipal claims)
        {
            var id = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var patientId = new Guid(id);
            var patient = await _unitOfWork.Patients.GetAsync(p => p.Id == patientId);

            if (patient == null)
            {
                throw new KeyNotFoundException("Incorrect jwt token or patient deleted");
            }
            _unitOfWork.Patients.Remove(patient);
            await _unitOfWork.SaveChangesAsync();

            return new PatientResponse
            {
                Id = patient.Id,
                UserName = patient.UserName,
                FullName = patient.FullName,
                Email = patient.Email,
                PhoneNumber = patient.PhoneNumber,
                Balance = patient.Balance,
                JoinDate = patient.JoinDate,
                Sex = patient.Sex,
                Address = patient.Address,
                Status = patient.Status
            };
        }

        public async Task<List<PatientResponse>> GetAll(ClaimsPrincipal claims)
        {
            var patients = await _unitOfWork.Patients.GetAllAsync();
            List<PatientResponse> result = new List<PatientResponse>();
            foreach (var item in patients)
            {
                result.Add(new PatientResponse
                {
                    Id = item.Id,
                    UserName = item.UserName,
                    Address = item.Address,
                    Balance = item.Balance,
                    Email = item.Email,
                    FullName = item.FullName,
                    JoinDate = item.JoinDate,
                    PhoneNumber = item.PhoneNumber,
                    Sex = item.Sex,
                    Status = item.Status
                });
            }
            return result;
        }

        public async Task<PatientResponse?> GetById(string code)
        {
            bool isValid = Guid.TryParse(code, out guid);
            if (!isValid)
            {
                throw new ArgumentException("Incorrect GUID format.");
            }

            var patient = await _unitOfWork.Patients.GetAsync(p => p.Id == guid);
            if (patient == null)
            {
                throw new NotFoundException("Patient not found.");
            }
            return new PatientResponse
            {
                Id = patient.Id,
                UserName = patient.UserName,
                FullName = patient.FullName,
                Email = patient.Email,
                PhoneNumber = patient.PhoneNumber,
                Balance = patient.Balance,
                JoinDate = patient.JoinDate,
                Sex = patient.Sex,
                Address = patient.Address,
                Status = patient.Status
            };
        }

        public async Task<PatientResponse?> Update(PatientRequest patientModel, ClaimsPrincipal claims)
        {
            var id = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var patientId = new Guid(id);
            var patient = await _unitOfWork.Patients.GetAsync(p => p.Id == patientId && p.Status.Equals("Active"));

            if (patient == null)
            {
                throw new KeyNotFoundException("Incorrect jwt token or patient deleted");
            }
            patient.UserName = patientModel.UserName.IsNullOrEmpty() ? patient.UserName : patientModel.UserName;
            patient.FullName = patientModel.FullName.IsNullOrEmpty() ? patient.FullName : patientModel.FullName;
            patient.Email = patientModel.Email.IsNullOrEmpty() ? patient.Email : patientModel.Email;
            patient.PhoneNumber = patientModel.PhoneNumber.IsNullOrEmpty() ? patient.PhoneNumber : patientModel.PhoneNumber;
            patient.Balance = patientModel.Balance == null || patientModel.Balance <= 0 ? patient.Balance : patientModel.Balance;
            patient.Address = patientModel.Address.IsNullOrEmpty() ? patient.Address : patientModel.Address;
            patient.Status = patientModel.Status.IsNullOrEmpty() ? patient.Status : patientModel.Status;
            patient.JoinDate = patientModel.JoinDate is null ? patient.JoinDate : patientModel.JoinDate;
            patient.Sex = patientModel.Sex.IsNullOrEmpty() ? patient.Sex : patientModel.Sex;

            _unitOfWork.Patients.Update(patient);
            await _unitOfWork.SaveChangesAsync();

            return new PatientResponse
            {
                Id = patient.Id,
                UserName = patient.UserName,
                FullName = patient.FullName,
                Email = patient.Email,
                PhoneNumber = patient.PhoneNumber,
                Balance = patient.Balance,
                JoinDate = patient.JoinDate,
                Sex = patient.Sex,
                Address = patient.Address,
                Status = patient.Status
            };
        }
    }
}
