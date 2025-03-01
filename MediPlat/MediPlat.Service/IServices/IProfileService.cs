using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using System.Security.Claims;

namespace MediPlat.Service.IServices
{
    public interface IProfileService
    {
        Task<ProfileResponse?> GetById(string code);
        Task<List<ProfileResponse>> GetAll(ClaimsPrincipal claims);
        Task<ProfileResponse?> Create(ProfileRequest ProductModel, ClaimsPrincipal claims);
        Task<ProfileResponse?> Update(string id, ProfileRequest ProductModel, ClaimsPrincipal claims);
        Task<ProfileResponse?> DeleteById(string id, ClaimsPrincipal claims);
    }
}
