using System;
using System.ComponentModel.DataAnnotations;

namespace MediPlat.Model.ResponseObject
{
    public class AppointmentSlotMedicineResponse
    {
        [Key]
        public Guid AppointmentSlotMedicineId { get; set; }
        public Guid AppointmentSlotId { get; set; }
        public Guid MedicineId { get; set; }
        public string Dosage { get; set; } = null!;
        public string? Instructions { get; set; }
        public short Quantity { get; set; }
        public MedicineResponse? Medicine { get; set; } = null!;
        public AppointmentSlotResponse? AppointmentSlot { get; set; } = null!;
    }
}
