using System;
using System.ComponentModel.DataAnnotations;

namespace MediPlat.Model.RequestObject
{
        public class TransactionRequest
        {
                [Required]
                public Guid PatientId { get; set; }

                [Required]
                public Guid DoctorId { get; set; }

                public Guid? SubId { get; set; }

                public Guid? AppointmentSlotId { get; set; }

                [Required]
                [MaxLength(500, ErrorMessage = "TransactionInfo cannot exceed 500 characters.")]
                public string? TransactionInfo { get; set; }

                [Required]
                [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
                public decimal Amount { get; set; }

                public DateTime? CreatedDate { get; set; } = DateTime.Now;

                [Required]
                [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
                public string Status { get; set; } = "Pending";
        }
}
