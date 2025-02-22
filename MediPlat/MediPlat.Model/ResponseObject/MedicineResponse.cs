using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MediPlat.Model.ResponseObject
{
    public class MedicineResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? DosageForm { get; set; }
        public string Strength { get; set; } = null!;
        public string? SideEffects { get; set; }

        [JsonIgnore]
        public virtual ICollection<AppointmentSlotMedicineResponse> AppointmentSlotMedicines { get; set; } = new List<AppointmentSlotMedicineResponse>();
    }
}
