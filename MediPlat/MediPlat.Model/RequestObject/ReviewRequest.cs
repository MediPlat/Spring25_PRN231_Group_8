using System;
using System.ComponentModel.DataAnnotations;

namespace MediPlat.Model.RequestObject
{
    public class ReviewRequest
    {
        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        public Guid SlotId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [MaxLength(1000, ErrorMessage = "Message cannot exceed 1000 characters.")]
        public string? Message { get; set; }

        public DateTime? CreatedDate { get; set; } = DateTime.Now;
    }
}
