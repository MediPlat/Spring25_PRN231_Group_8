using System;
using System.Collections.Generic;

namespace MediPlat.Model.Model;

public partial class Experience
{
    public Guid Id { get; set; }

    public Guid? SpecialtyId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Certificate { get; set; }

    public Guid? DoctorId { get; set; }

    public string? Status { get; set; }

    public virtual Doctor? Doctor { get; set; }

    public virtual Specialty? Specialty { get; set; }
}
