using System;
using System.ComponentModel.DataAnnotations;

namespace MediPlat.Model.RequestObject
{
    public class DoctorSubscriptionRequest
    {
        [Required]
        public Guid SubscriptionId { get; set; }

        [Required]
        [Range(1, 255, ErrorMessage = "EnableSlot must be between 1 and 255.")]
        public byte EnableSlot { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime EndDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
