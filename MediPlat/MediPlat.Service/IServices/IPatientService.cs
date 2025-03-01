using MediPlat.Model.RequestObject.Auth;
using MediPlat.Model.RequestObject.Patient;
using MediPlat.Model.ResponseObject.Patient;
using System.Security.Claims;

namespace MediPlat.Service.IServices
{
    public interface IPatientService
    {
        Task<PatientResponse?> GetById(string code);
        Task<List<PatientResponse>> GetAll(ClaimsPrincipal claims);
        Task<PatientResponse?> Create(PatientRequest ProductModel, ClaimsPrincipal claims);
        Task<PatientResponse?> Update(string id, PatientRequest ProductModel, ClaimsPrincipal claims);
        Task<PatientResponse?> DeleteById(string id, ClaimsPrincipal claims);
        Task<PatientResponse?> ChangePassword(ClaimsPrincipal claims, ChangePasswordRequest changePasswordRequest);

        IQueryable<PatientResponse> GetAllAsQueryable(ClaimsPrincipal claims);

    }
}
