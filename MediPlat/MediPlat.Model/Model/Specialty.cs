﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MediPlat.Model.Model;

public partial class Specialty
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }
    public virtual ICollection<Experience> Experiences { get; set; } = new List<Experience>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
