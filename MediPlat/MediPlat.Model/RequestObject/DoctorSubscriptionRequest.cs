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
        [Range(1, 1000, ErrorMessage = "EnableSlot must be between 1 and 1000.")]
        public short? EnableSlot { get; set; }
        public Guid? DoctorId { get; set; }
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; } = DateTime.Now;
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        [EnumDataType(typeof(DoctorSubscriptionStatus))]
        public string? Status { get; set; } = "Active";
    }
}
