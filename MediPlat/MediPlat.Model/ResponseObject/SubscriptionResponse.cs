using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MediPlat.Model.ResponseObject
{
    public class SubscriptionResponse
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string? Name { get; set; }

        [Required]
        public decimal? Price { get; set; }

        public byte EnableSlot { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdateDate { get; set; }

        [JsonIgnore]
        public virtual ICollection<DoctorSubscriptionResponse> DoctorSubscriptions { get; set; } = new List<DoctorSubscriptionResponse>();
    }
}
