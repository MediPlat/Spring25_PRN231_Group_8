using System;

namespace MediPlat.Model.ResponseObject
{
    public class AppointmentSlotMedicineResponse
    {
        public Guid AppointmentSlotMedicineId { get; set; }
        public Guid AppointmentSlotId { get; set; }
        public Guid MedicineId { get; set; }
        public string Dosage { get; set; } = null!;
        public string? Instructions { get; set; }
        public short Quantity { get; set; }
        public MedicineResponse Medicine { get; set; } = null!;
        public AppointmentSlotResponse AppointmentSlot { get; set; } = null!;
    }
}
