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
        Task CreateAppointmentSlot(AppointmentSlotRequest appointmentSlotRequest);
        Task UpdateAppointmentSlot(Guid id, AppointmentSlotRequest appointmentSlotRequest);
        Task DeleteAppointmentSlot(Guid slotId);
        Task <IEnumerable<AppointmentSlotResponse?>> GetAppointmentSlotBySlotID(Guid slotId);
        Task<IEnumerable<AppointmentSlotResponse?>> GetAppointmentSlotByProfileID(Guid profileId);
    }
}
