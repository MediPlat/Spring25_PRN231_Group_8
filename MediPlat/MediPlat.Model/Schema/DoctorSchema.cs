using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Model.Schema
{
    public class DoctorSchema
    {

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public decimal? FeePerHour { get; set; }

        public string? Degree { get; set; }

        public string? AcademicTitle { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
