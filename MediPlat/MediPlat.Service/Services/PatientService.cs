using MediPlat.Model;
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
        private readonly IPatientRepository _patientRepository;
        public PatientService(IPatientRepository patientRepository) 
        {
            _patientRepository = patientRepository;
        }
        public async Task<PatientResponse?> Create(PatientRequest patientModel, ClaimsPrincipal claims)
        {
            try
            {
                Guid guid = Guid.NewGuid(); 
                _patientRepository.Add(new Patient
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PatientResponse?> DeleteById(string id)
        {
            guid = new Guid(id);
            var patient = await _patientRepository.GetAsync(p => p.Id == guid);

            if (patient == null)
            {
                return null;
            }
            try
            {
                _patientRepository.Remove(patient);

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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<PatientResponse>> GetAll(ClaimsPrincipal claims)
        {
            var patients = await _patientRepository.GetAllAsync();
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
            try
            {
                guid = new Guid(code);
                var patient = await _patientRepository.GetAsync(p => p.Id == guid);

                if (patient == null)
                {
                    return null;
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PatientResponse?> Update(string id, PatientRequest patientModel, ClaimsPrincipal claims)
        {
            guid = new Guid(id);
            var patient = await _patientRepository.GetAsync(p => p.Id == guid);

            if (patient == null)
            {
                return null;
            }
            try
            {
                patient.UserName = patientModel.UserName.IsNullOrEmpty() ? patient.UserName : patientModel.UserName;
                patient.FullName = patientModel.FullName.IsNullOrEmpty() ? patient.FullName : patientModel.FullName;
                patient.Email = patientModel.Email.IsNullOrEmpty() ? patient.Email : patientModel.Email;
                patient.PhoneNumber = patientModel.PhoneNumber.IsNullOrEmpty() ? patient.PhoneNumber : patientModel.PhoneNumber;
                patient.Balance = patientModel.Balance == 0 ? patient.Balance : patientModel.Balance;
                patient.Address = patientModel.Address.IsNullOrEmpty() ? patient.Address : patientModel.Address;
                patient.Status = patientModel.Status.IsNullOrEmpty() ? patient.Status : patientModel.Status;
                patient.JoinDate = patientModel.JoinDate is null ? patient.JoinDate : patientModel.JoinDate;
                patient.Sex = patientModel.Sex.IsNullOrEmpty() ? patient.Sex : patientModel.Sex;

                _patientRepository.Update(patient);

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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
