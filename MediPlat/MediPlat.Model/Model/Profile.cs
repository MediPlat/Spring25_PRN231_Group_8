using System;
using System.Collections.Generic;

namespace MediPlat.Model.Model;

public partial class Profile
{
    public Guid Id { get; set; }

    public Guid? PatientId { get; set; }

    public string? FullName { get; set; }

    public string? Sex { get; set; }

    public DateTime? Dob { get; set; }

    public string? Address { get; set; }

    public DateTime? JoinDate { get; set; }

    public string? PhoneNumber { get; set; }

    public virtual ICollection<AppointmentSlot> AppointmentSlots { get; set; } = new List<AppointmentSlot>();

    public virtual Patient? Patient { get; set; }
}
