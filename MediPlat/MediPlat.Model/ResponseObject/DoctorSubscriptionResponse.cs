using MediPlat.Model.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MediPlat.Model.ResponseObject
{
    public class DoctorSubscriptionResponse
    {
        public Guid Id { get; set; }

        public Guid SubscriptionId { get; set; }

        public short EnableSlot { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime? UpdateDate { get; set; }
        public string? Status { get; set; }

        public Guid DoctorId { get; set; }

        public DoctorResponse Doctor { get; set; } = null!;
        public SubscriptionResponse Subscription { get; set; } = null!;
    }
}
