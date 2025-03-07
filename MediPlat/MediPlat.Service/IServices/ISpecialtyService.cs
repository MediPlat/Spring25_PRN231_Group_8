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
    }
}
