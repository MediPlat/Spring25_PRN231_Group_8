using System;
using System.ComponentModel.DataAnnotations;

namespace MediPlat.Model.RequestObject
{
        public class DoctorRequest
        {
                [Required]
                [MaxLength(255, ErrorMessage = "UserName cannot exceed 255 characters.")]
                public string? UserName { get; set; }

                [Required]
                [MaxLength(255, ErrorMessage = "FullName cannot exceed 255 characters.")]
                public string? FullName { get; set; }

                [Required]
                [EmailAddress(ErrorMessage = "Invalid email format.")]
                public string? Email { get; set; }

                [Required]
                [MinLength(6, ErrorMessage = "Password must have at least 6 characters.")]
                public string? Password { get; set; }

                public string? AvatarUrl { get; set; }

                [Range(0, double.MaxValue, ErrorMessage = "Balance cannot be negative.")]
                public decimal? Balance { get; set; }

                [Range(0, double.MaxValue, ErrorMessage = "FeePerHour must be greater than or equal to 0.")]
                public decimal? FeePerHour { get; set; }

                [MaxLength(255, ErrorMessage = "Degree cannot exceed 255 characters.")]
                public string? Degree { get; set; }

                [MaxLength(255, ErrorMessage = "AcademicTitle cannot exceed 255 characters.")]
                public string? AcademicTitle { get; set; }

                public DateTime? JoinDate { get; set; } = DateTime.Now;

                [MaxLength(50, ErrorMessage = "PhoneNumber cannot exceed 50 characters.")]
                public string? PhoneNumber { get; set; }

                [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
                public string? Status { get; set; } = "Active";
        }
}
