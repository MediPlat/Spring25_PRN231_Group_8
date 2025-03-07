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
    public interface ISlotService
    {
        IQueryable<SlotResponse> GetSlot();
        Task<SlotResponse?> GetSlotByID(Guid slotId);
        Task CreateSlot(SlotRequest slotRequest);
        Task UpdateSlot(SlotRequest slotRequest);
        Task DeleteSlot(Guid slotId);

    }
}
