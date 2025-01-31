using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Model.RequestObject
{
    public class DoctorSubscriptionRequest
    {
        public Guid? SubscriptionId { get; set; }

        public byte? EnableSlot { get; set; }

        public string? Description { get; set; }

        public Guid? DoctorId { get; set; }
    }
}
