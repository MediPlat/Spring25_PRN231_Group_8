using System;
using System.ComponentModel.DataAnnotations;

namespace MediPlat.Model.RequestObject
{
    public class ServiceRequest
    {
        [Required]
        public Guid SpecialtyId { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "Title cannot exceed 255 characters.")]
        public string? Title { get; set; }

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }
    }
}
