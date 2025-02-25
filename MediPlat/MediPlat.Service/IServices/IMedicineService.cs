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
        Task<MedicineResponse> AddMedicineAsync(MedicineRequest request);
        Task<MedicineResponse> UpdateMedicineAsync(Guid id, MedicineRequest request);
        Task<MedicineResponse> DeactivateMedicineAsync(Guid id);
    }
}
