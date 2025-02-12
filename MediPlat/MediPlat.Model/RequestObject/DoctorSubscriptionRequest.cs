using System;
using System.ComponentModel.DataAnnotations;

namespace MediPlat.Model.RequestObject
{
    public enum DoctorSubscriptionStatus
    {
        [Display(Name = "Active")]
        Active,

        [Display(Name = "Inactive")]
        Expired
    }
    public class DoctorSubscriptionRequest
    {
        [Required]
        public Guid SubscriptionId { get; set; }

        [Range(1, 255, ErrorMessage = "EnableSlot must be between 1 and 255.")]
        public short EnableSlot { get; set; }
        [Required]
        public Guid DoctorId { get; set; }
        public DateTime? StartDate { get; set; } = DateTime.Now;

        public DateTime? EndDate { get; set; }

        public DateTime? UpdateDate { get; set; }
        [Required]
        [EnumDataType(typeof(DoctorSubscriptionStatus))]
        public string Status { get; set; } = "Active";
    }
}
