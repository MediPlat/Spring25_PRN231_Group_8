using System;
using System.ComponentModel.DataAnnotations;

namespace MediPlat.Model.RequestObject
{
        public class AppointmentSlotRequest
        {
                [Required]
                public Guid SlotId { get; set; }

                //[Required]
                public Guid PatientId { get; set; }

                [Required]
                [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
                public string Status { get; set; } = null!;

                public DateTime? CreatedDate { get; set; } = DateTime.Now;

                [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
                public string? Notes { get; set; }
        }
}
