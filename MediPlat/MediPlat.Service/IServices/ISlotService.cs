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
        Task<string> CreateSlot(SlotRequest slotRequest);    }
}
