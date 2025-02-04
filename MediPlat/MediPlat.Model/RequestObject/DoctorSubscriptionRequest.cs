using System;
using System.ComponentModel.DataAnnotations;

namespace MediPlat.Model.RequestObject
{
    public class DoctorSubscriptionRequest
    {
        [Required]
        public Guid SubscriptionId { get; set; }

        [Required]
        public byte EnableSlot { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public Guid DoctorId { get; set; }
    }
}