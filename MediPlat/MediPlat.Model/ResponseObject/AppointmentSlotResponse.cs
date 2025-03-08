using MediPlat.Model.ResponseObject.Patient;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MediPlat.Model.ResponseObject
{
    public class AppointmentSlotResponse
    {
        public Guid Id { get; set; }

        public Guid SlotId { get; set; }

        public string Status { get; set; } = null!;

        public DateTime? CreatedDate { get; set; }

        public string? Notes { get; set; }

        [JsonIgnore]
        public virtual List<AppointmentSlotMedicineResponse> AppointmentSlotMedicines { get; set; } = new List<AppointmentSlotMedicineResponse>();

        [JsonIgnore]
        public virtual List<TransactionResponse> Transactions { get; set; } = new List<TransactionResponse>();

        public virtual SlotResponse? Slot { get; set; }
    }
}
