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
        [Required]
        [Range(1, 255, ErrorMessage = "EnableSlot must be between 1 and 255.")]
        public short EnableSlot { get; set; }
        [Required]
        public Guid DoctorId { get; set; }
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; } = DateTime.Now;
        [DataType(DataType.Date)]
        [CustomValidation(typeof(DoctorSubscriptionRequest), "ValidateSubscriptionDates")]
        public DateTime? EndDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        [Required]
        [EnumDataType(typeof(DoctorSubscriptionStatus))]
        public string Status { get; set; } = "Active";
        public static ValidationResult ValidateSubscriptionDates(DateTime? endDate, ValidationContext context)
        {
            var instance = (DoctorSubscriptionRequest)context.ObjectInstance;
            if (endDate.HasValue && instance.StartDate.HasValue && endDate <= instance.StartDate)
            {
                return new ValidationResult("EndDate must be greater than StartDate.");
            }
            return ValidationResult.Success;
        }
    }
}
