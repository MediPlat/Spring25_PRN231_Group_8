using System;
using System.ComponentModel.DataAnnotations;

namespace MediPlat.Model.RequestObject
{
    public class SubscriptionRequest
    {
        [Required]
        [MaxLength(255)]
        public string? Title { get; set; }

        [Required]
        [MaxLength(255)]
        public string? Name { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }

        [Required]
        [Range(1, 255, ErrorMessage = "EnableSlot must be between 1 and 255.")]
        public byte EnableSlot { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
