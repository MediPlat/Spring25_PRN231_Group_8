using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MediPlat.Model.ResponseObject
{
    public class SubscriptionResponse
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public decimal? Price { get; set; }

        public byte EnableSlot { get; set; }

        public string? Description { get; set; }

        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdateDate { get; set; }

        [JsonIgnore]
        public virtual ICollection<DoctorSubscriptionResponse> DoctorSubscriptions { get; set; } = new List<DoctorSubscriptionResponse>();
    }
}
