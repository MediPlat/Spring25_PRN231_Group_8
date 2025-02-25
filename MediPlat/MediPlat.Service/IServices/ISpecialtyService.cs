using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;

namespace MediPlat.Service.IServices
{
    public interface ISpecialtyService
    {
        List<SpecialtyResponse> GetAllSpecialties();
        Task<SpecialtyResponse?> GetSpecialtyByIdAsync(Guid id);
        Task<SpecialtyResponse> AddSpecialtyAsync(SpecialtyRequest request);
        Task<SpecialtyResponse> UpdateSpecialtyAsync(Guid id, SpecialtyRequest request);
        Task<bool> DeleteSpecialtyAsync(Guid id);
    }
}
