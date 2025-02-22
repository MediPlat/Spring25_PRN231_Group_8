using MediPlat.Model.RequestObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Service.IServices
{
    public interface ISlotService
    {
        Task<string> CreateSlot(SlotRequest slotRequest);
        Task<string> UpdateSlot(SlotRequest slotRequest);
        Task<string> GetSlotByID(Guid slotId);
        Task<string> GetSlotByDocorID(Guid slotId);
    }
}
