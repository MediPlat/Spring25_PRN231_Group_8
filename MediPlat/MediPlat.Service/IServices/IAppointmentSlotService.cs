using MediPlat.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Service.IServices
{
    public interface IAppointmentSlotService
    {
        IQueryable<AppointmentSlotResponse> GetAppointmentSlot();
        Task<List<AppointmentSlotResponse>> GetAppointmentSlotsForDoctorAsync(Guid doctorId);
        Task<List<AppointmentSlotResponse>> GetAppointmentSlotsForPatientAsync(Guid profileId);
        Task<AppointmentSlotResponse> GetAppointmentSlotByIdForDoctorAsync(Guid doctorId, Guid appointmentSlotId);
        Task<AppointmentSlotResponse> GetAppointmentSlotByIdForPatientAsync(Guid profileId, Guid appointmentSlotId);
        Task<AppointmentSlotResponse?> CreateAppointmentSlot(AppointmentSlotRequest appointmentSlotRequest);
        Task<AppointmentSlotResponse> UpdateAppointmentSlot(Guid id, AppointmentSlotRequest appointmentSlotRequest);
        Task DeleteAppointmentSlot(Guid slotId);
    }
}
