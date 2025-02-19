using MediPlat.Model.ResponseObject.Patient;
using System;
using System.Text.Json.Serialization;

namespace MediPlat.Model.ResponseObject
{
    public class TransactionResponse
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }

        public Guid DoctorId { get; set; }

        public Guid? SubId { get; set; }

        public Guid? AppointmentSlotId { get; set; }

        public string? TransactionInfo { get; set; }

        public decimal Amount { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string Status { get; set; }

        public virtual SubscriptionResponse? Sub { get; set; }

        public virtual AppointmentSlotResponse? AppointmentSlot { get; set; }

        public virtual DoctorResponse? Doctor { get; set; }

        public virtual PatientResponse? Patient { get; set; }
    }
}
