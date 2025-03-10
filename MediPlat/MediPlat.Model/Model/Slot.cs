﻿using System;
using System.Collections.Generic;

namespace MediPlat.Model.Model;

public partial class Slot
{
    public Guid Id { get; set; }

    public Guid? DoctorId { get; set; }

    public Guid? ServiceId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public DateTime? Date { get; set; }

    public decimal? SessionFee { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<AppointmentSlot> AppointmentSlots { get; set; } = new List<AppointmentSlot>();

    public virtual Doctor? Doctor { get; set; }

    public virtual Review? Review { get; set; }

    public virtual Service? Service { get; set; }
}
