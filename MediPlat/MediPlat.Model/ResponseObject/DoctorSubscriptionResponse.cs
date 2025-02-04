using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MediPlat.Model.ResponseObject
{
    public class DoctorSubscriptionResponse
    {
        public Guid Id { get; set; }

        public Guid? SubscriptionId { get; set; }

        public byte? EnableSlot { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public Guid? DoctorId { get; set; }

        [JsonIgnore]  // Avoid circular reference issues when serializing
        public virtual Doctor? Doctor { get; set; }

        [JsonIgnore]  // Avoid circular reference issues
        public virtual Subscription? Subscription { get; set; }
    }
}
