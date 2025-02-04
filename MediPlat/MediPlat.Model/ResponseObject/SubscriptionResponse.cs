using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MediPlat.Model.ResponseObject
{
    public class SubscriptionResponse
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public byte? EnableSlot { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        [JsonIgnore] // Prevent circular reference
        public virtual ICollection<DoctorSubscription> DoctorSubscriptions { get; set; } = new List<DoctorSubscription>();

        [JsonIgnore]
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
