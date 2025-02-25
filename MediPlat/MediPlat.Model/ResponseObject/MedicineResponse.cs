using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MediPlat.Model.ResponseObject
{
    public class MedicineResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? DosageForm { get; set; }
        public string Strength { get; set; } = null!;
        public string? SideEffects { get; set; }
        public string Status { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<AppointmentSlotMedicineResponse> AppointmentSlotMedicines { get; set; } = new List<AppointmentSlotMedicineResponse>();
    }
}
