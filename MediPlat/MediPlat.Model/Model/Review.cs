using System;
using System.Collections.Generic;

namespace MediPlat.Model.Model;

public partial class Review
{
    public Guid Id { get; set; }

    public int? Rating { get; set; }

    public string? Message { get; set; }

    public Guid? PatientId { get; set; }

    public Guid? DoctorId { get; set; }

    public Guid? SlotId { get; set; }

    public virtual Doctor? Doctor { get; set; }

    public virtual Patient? Patient { get; set; }

    public virtual Slot? Slot { get; set; }
}
