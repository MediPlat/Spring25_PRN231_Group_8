using System;
using System.Collections.Generic;

namespace MediPlat.Model.Model;

public partial class Specialty
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Experience> Experiences { get; set; } = new List<Experience>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();

    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();
}
