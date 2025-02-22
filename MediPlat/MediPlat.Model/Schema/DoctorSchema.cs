using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Model.Schema
{
    public class DoctorSchema
    {
        [Required]
        public string? FullName { get; set; }
        [Required]
        public string? UserName { get; set; }
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [RegularExpression(@"^[^@\s]+@gmail\.com$", ErrorMessage = "Email must end with @gmail.com")]
        public string? Email { get; set; }
        [Range(1000, 200000, ErrorMessage = "FeePerHour must be between 1000 and 200000")]
        public decimal? FeePerHour { get; set; }
        [Required]
        public string? Degree { get; set; }
        [Required]
        public string? AcademicTitle { get; set; }
        
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be 10 digits and start with a number")]
        public string? PhoneNumber { get; set; }
    }

}
