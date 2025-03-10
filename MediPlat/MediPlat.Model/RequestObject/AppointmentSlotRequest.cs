using System;
using System.ComponentModel.DataAnnotations;

namespace MediPlat.Model.RequestObject
{
    public class AppointmentSlotRequest
    {
        public Guid? SlotId { get; set; }

        public Guid? ProfileId { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public string Status { get; set; } = "Pending";

        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string? Notes { get; set; }
        public ICollection<AppointmentSlotMedicineRequest> Medicines { get; set; } = new List<AppointmentSlotMedicineRequest>();
    }
}
