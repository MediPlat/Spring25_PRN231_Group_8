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
                public Guid? SpecialtyId { get; set; }

                [MaxLength(255)]
                public string Title { get; set; }

                [MaxLength(1000)]
                public string? Description { get; set; }

                [MaxLength(1000)]

                public string? Certificate { get; set; }

                [RegularExpression("^(Active|Suspended)$", ErrorMessage = "Status chỉ có thể là 'Active' hoặc 'Suspended'.")]
                public string? Status { get; set; }

                public Guid? DoctorId { get; set; }

        }
}
