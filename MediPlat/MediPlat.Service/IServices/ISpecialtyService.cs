using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MediPlat.Service.IServices
{
    public interface ISpecialtyService
    {
        IQueryable<SpecialtyResponse> GetAllSpecialties();
        Task<SpecialtyResponse?> GetSpecialtyByIdAsync(Guid id);
        Task<SpecialtyResponse> AddSpecialtyAsync(SpecialtyRequest request);
        Task<SpecialtyResponse> UpdateSpecialtyAsync(Guid id, SpecialtyRequest request);
        Task<bool> DeleteSpecialtyAsync(Guid id);
    }
}
