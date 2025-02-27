using MediPlat.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MediPlat.Model.ResponseObject
{
    public class ExperienceResponse
    {
        public Guid Id { get; set; }

        public Guid? SpecialtyId { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }

        public string? Certificate { get; set; }

        public string? Status { get; set; }

        public Guid? DoctorId { get; set; }

        public virtual DoctorResponse Doctor { get; set; }

        public virtual SpecialtyResponse Specialty { get; set; }
    }
}
