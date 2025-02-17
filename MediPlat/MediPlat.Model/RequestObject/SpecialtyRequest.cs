using System;
using System.ComponentModel.DataAnnotations;

namespace MediPlat.Model.RequestObject
{
    public class SpecialtyRequest
    {
        [Required]
        [MaxLength(255, ErrorMessage = "Name cannot exceed 255 characters.")]
        public string Name { get; set; } = null!;

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }
    }
}
