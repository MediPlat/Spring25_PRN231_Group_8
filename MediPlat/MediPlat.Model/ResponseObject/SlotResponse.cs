using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MediPlat.Model.ResponseObject
{
    public class SlotResponse
    {
        public Guid Id { get; set; }

        public Guid DoctorId { get; set; }

        public Guid ServiceId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime Date { get; set; }

        public decimal SessionFee { get; set; }

        public string Status { get; set; }

        public DoctorResponse? Doctor { get; set; }

        public ServiceResponse? Service { get; set; }

        [JsonIgnore]
        public virtual List<AppointmentSlotResponse> AppointmentSlots { get; set; } = new List<AppointmentSlotResponse>();

        [JsonIgnore]
        public virtual List<ReviewResponse> Reviews { get; set; } = new List<ReviewResponse>();
    }
}
