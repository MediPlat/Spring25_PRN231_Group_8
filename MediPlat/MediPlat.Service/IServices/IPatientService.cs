using MediPlat..ResponseObject.Patient;
using MediPlat.Model;
using MediPlat.Model.RequestObject.Patient;
using MediPlat.Model.ResponseObject.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Service.IServices
{
    public interface IPatientService
    {
        Task<PatientResponse?> GetById(string code);
        Task<List<PatientResponse>> GetAll(ClaimsPrincipal claims);
        Task<PatientResponse?> Create(PatientRequest ProductModel, ClaimsPrincipal claims);
        Task<PatientResponse?> Update(string id, PatientRequest ProductModel, ClaimsPrincipal claims);
        Task<PatientResponse?> DeleteById(string id);
    }
}
