using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MediPlat.Model.Model;
using MediPlat.Model.ResponseObject.Patient;

namespace MediPlat.Model.ResponseObject
{
    public class AppointmentSlotMedicineResponse
    {
        public Guid AppointmentSlotId { get; set; }
        public Guid MedicineId { get; set; }
        public Guid PatientId { get; set; }
        public string Dosage { get; set; } = null!;
        public string? Instructions { get; set; }
        public short Quantity { get; set; }

        [JsonIgnore]
        public virtual AppointmentSlotResponse AppointmentSlot { get; set; } = null!;
        [JsonIgnore]
        public virtual MedicineResponse Medicine { get; set; } = null!;
        [JsonIgnore]
        public virtual PatientResponse Patient { get; set; }
    }
}
