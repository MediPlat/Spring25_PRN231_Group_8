using System;
using System.Collections.Generic;

namespace MediPlat.Model.Model;

public partial class Patient
{
    public Guid Id { get; set; }

    public string? UserName { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? PhoneNumber { get; set; }

    public decimal? Balance { get; set; }

    public DateTime? JoinDate { get; set; }

    public string? Sex { get; set; }

    public string? Address { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<AppointmentSlotMedicine> AppointmentSlotMedicines { get; set; } = new List<AppointmentSlotMedicine>();

    public virtual ICollection<AppointmentSlot> AppointmentSlots { get; set; } = new List<AppointmentSlot>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
