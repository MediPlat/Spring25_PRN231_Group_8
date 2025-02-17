using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Model.RequestObject
{
    public class AppointmentSlotMedicineRequest
    {
        [Required]
        public Guid AppointmentSlotId { get; set; }
        [Required]
        public Guid MedicineId { get; set; }
        [Required]
        public Guid PatientId { get; set; }
        [Required]
        [MinLength(5, ErrorMessage = "Dosage must have at least 5 characters.")]
        [MaxLength(255, ErrorMessage = "Dosage cannot exceed 255 characters.")]
        public string Dosage { get; set; } = null!;
        public string? Instructions { get; set; }
        [Required]
        [Range(1, short.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public short Quantity { get; set; }
    }
}
