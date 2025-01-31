using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Model.ResponseObject
{
    public class DoctorSubscriptionResponse
    {
        public Guid Id { get; set; }

        public Guid? SubscriptionId { get; set; }

        public byte? EnableSlot { get; set; }

        public string? Description { get; set; }

        public Guid? DoctorId { get; set; }

        public virtual Doctor? Doctor { get; set; }

        public virtual Subscription? Subscription { get; set; }
    }
}
