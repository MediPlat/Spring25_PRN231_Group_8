using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Service.IServices
{
    public interface IMedicineService
    {
        IQueryable<MedicineResponse> GetAllMedicines();
        Task<MedicineResponse?> GetMedicineByIdAsync(Guid id);
        Task AddMedicineAsync(MedicineRequest request);
        Task UpdateMedicineAsync(Guid id, MedicineRequest request);
        Task DeleteMedicineAsync(Guid id);
    }
}
