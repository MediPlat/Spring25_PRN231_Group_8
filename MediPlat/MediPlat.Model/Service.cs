using System;
using System.Collections.Generic;

namespace MediPlat.Model;

public partial class Service
{
    public Guid Id { get; set; }

    public Guid? SpecialtyId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();

    public virtual Specialty? Specialty { get; set; }
}
