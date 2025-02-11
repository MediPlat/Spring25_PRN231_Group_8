using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Model.RequestObject
{
    public class ExperienceRequest
    {
        [Required]
        public Guid? SpecialtyId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(1000)]

        public string? Certificate { get; set; }

        public Guid? DoctorId { get; set; }

    }
}
