using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Model.RequestObject.Auth
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [MaxLength(255, ErrorMessage = "Email is too long")]
        public required string Email { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "Password is too long")]
        public required string Password { get; set; }
    }
}
