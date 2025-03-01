using AutoMapper;
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
                throw new KeyNotFoundException("Incorrect jwt token or patient deleted");
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
                Email = patientModel.Email,
                Balance = patientModel.Balance,
                Status = "Active",
                Password = patientModel.Password,
            });
            await _unitOfWork.SaveChangesAsync();
            return _mapper?.Map<PatientResponse?>(await _unitOfWork.Patients.GetAsync(p => p.Id.Equals(guid)));
        }

        public async Task<PatientResponse?> DeleteById(string id, ClaimsPrincipal claims)
        {
            string pid = string.Empty;
            if(claims.FindFirst(ClaimTypes.Role)?.Value is "Admin")
            {
                pid = id;
            }
            else
            {
                pid = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

            var patientId = new Guid(pid);
            var patient = await _unitOfWork.Patients.GetAsync(p => p.Id == patientId, p => p.Profiles);
            if (patient == null)
            {
                throw new KeyNotFoundException("Incorrect jwt token or patient deleted");
            }
            patient.Status = "Suspended";
            _unitOfWork.Patients.Update(patient);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PatientResponse>(patient);
        }

        public async Task<List<PatientResponse>> GetAll(ClaimsPrincipal claims)
        {
            var patients = await _unitOfWork.Patients.GetAllAsync(p => p.Profiles);
            //List<PatientResponse> result = new List<PatientResponse>();
            //foreach (var item in patients)
            //{
            //    result.Add(new PatientResponse
            //    {
            //        Id = item.Id,
            //        UserName = item.UserName,
            //        Balance = item.Balance,
            //        Email = item.Email,
            //        Status = item.Status
            //    });
            //}
            //return result;

            return _mapper.Map<List<PatientResponse>>(patients);
        }

        public async Task<PatientResponse?> GetById(string code)
        {
            bool isValid = Guid.TryParse(code, out guid);
            if (!isValid)
            {
                throw new ArgumentException("Incorrect GUID format.");
            }

            var patient = await _unitOfWork.Patients.GetAsync(p => p.Id == guid, p => p.Profiles);
            if (patient == null)
            {
                throw new KeyNotFoundException("Patient not found.");
            }
            return _mapper.Map<PatientResponse>(patient);
        }

        public async Task<PatientResponse?> Update(string id, PatientRequest patientModel, ClaimsPrincipal claims)
        {
            string pid = string.Empty;
            if (claims.FindFirst(ClaimTypes.Role)?.Value is "Admin")
            {
                pid = id;
            }
            else
            {
                pid = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

            var patientId = new Guid(pid);
            var patient = await _unitOfWork.Patients.GetAsync(p => p.Id == patientId && p.Status.Equals("Active"), p => p.Profiles);

            if (patient == null)
            {
                throw new KeyNotFoundException("Incorrect jwt token or patient deleted");
            }

            patient.UserName = patientModel.UserName.IsNullOrEmpty() ? patient.UserName : patientModel.UserName;
            patient.Email = patientModel.Email.IsNullOrEmpty() ? patient.Email : patientModel.Email;
            patient.Balance = patientModel.Balance == null || patientModel.Balance <= 0 ? patient.Balance : patientModel.Balance;
            patient.Status = patientModel.Status.IsNullOrEmpty() ? patient.Status : patientModel.Status;

            _unitOfWork.Patients.Update(patient);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PatientResponse>(patient);
        }
        public IQueryable<PatientResponse> GetAllAsQueryable(ClaimsPrincipal claims)
        {
            var patients = _unitOfWork.Patients.GetAll().ToList();
            return _mapper.Map<List<PatientResponse>>(patients).AsQueryable();
        }
    }
}
