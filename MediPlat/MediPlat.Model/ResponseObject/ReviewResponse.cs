using MediPlat.Model.ResponseObject.Patient;
using System;
using System.Text.Json.Serialization;

namespace MediPlat.Model.ResponseObject
{
    public class ReviewResponse
    {
        public Guid Id { get; set; }

        public int Rating { get; set; }

        public string? Message { get; set; }

        public Guid PatientId { get; set; }

        public Guid DoctorId { get; set; }

        public Guid SlotId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public virtual DoctorResponse? Doctor { get; set; }

        public virtual PatientResponse? Patient { get; set; }

        [JsonIgnore]
        public virtual SlotResponse? Slot { get; set; }
    }
}
