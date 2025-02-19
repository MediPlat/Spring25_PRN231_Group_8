using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Service.IServices
{
    public interface IAppointmentSlotMedicineService
    {
        IQueryable<AppointmentSlotMedicineResponse> GetAllAppointmentSlotMedicines();
        Task<AppointmentSlotMedicineResponse?> GetAppointmentSlotMedicineByIdAsync(Guid appointmentSlotId, Guid medicineId, Guid patientId);
        Task AddAppointmentSlotMedicineAsync(AppointmentSlotMedicineRequest request);
        Task UpdateAppointmentSlotMedicineAsync(Guid appointmentSlotId, Guid medicineId, Guid patientId, AppointmentSlotMedicineRequest request);
        Task DeleteAppointmentSlotMedicineAsync(Guid appointmentSlotId, Guid medicineId, Guid patientId);
    }
}
