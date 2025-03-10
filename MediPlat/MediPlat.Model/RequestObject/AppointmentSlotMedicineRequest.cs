using System.ComponentModel.DataAnnotations;

namespace MediPlat.Model.RequestObject
{
    public class AppointmentSlotMedicineRequest
    {
        public Guid AppointmentSlotId { get; set; }
        [Required]
        public Guid MedicineId { get; set; }
        [Required(ErrorMessage = "Liều lượng không được để trống.")]
        [MinLength(1, ErrorMessage = "Liều lượng phải có ít nhất 1 ký tự.")]
        [MaxLength(50, ErrorMessage = "Liều lượng không được quá 50 ký tự.")]
        public string Dosage { get; set; } = "";
        public string? Instructions { get; set; }
        [Required]
        [Range(1, short.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public short Quantity { get; set; }
    }
}
