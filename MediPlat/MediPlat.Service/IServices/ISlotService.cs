using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;

namespace MediPlat.Service.IServices
{
    public interface ISlotService
    {
        IQueryable<SlotResponse> GetAllSlot();
        Task<SlotResponse?> GetSlotByID(Guid slotId);
        Task CreateSlot(SlotRequest slotRequest);
        Task UpdateSlot(SlotRequest slotRequest);
        Task DeleteSlot(Guid slotId);
        Task<IEnumerable<SlotResponse?>> GetSlotByDoctorID(Guid doctorId);
        Task<IEnumerable<SlotResponse?>> GetSlotByServiceID(Guid serviceId);
    }
}
