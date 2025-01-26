using MediPlat.Model.RequestObject.Patient;
using MediPlat.Model.ResponseObject.Patient;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IServices;
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
        public Task<PatientResponse?> Create(PatientRequest ProductModel, ClaimsPrincipal claims)
        {
            throw new NotImplementedException();
        }

        public Task<PatientResponse?> DeleteById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<PatientResponse>> GetAll(ClaimsPrincipal claims)
        {
            throw new NotImplementedException();
        }

        public async Task<PatientResponse?> GetById(string code)
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

        public Task<PatientResponse?> Update(string id, PatientRequest ProductModel, ClaimsPrincipal claims)
        {
            throw new NotImplementedException();
        }
    }
}
