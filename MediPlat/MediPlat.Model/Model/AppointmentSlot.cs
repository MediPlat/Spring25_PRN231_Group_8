using System;
using System.Collections.Generic;

namespace MediPlat.Model.Model;

public partial class AppointmentSlot
{
    public Guid Id { get; set; }

    public Guid? SlotId { get; set; }

    public Guid? PatientId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<AppointmentSlotMedicine> AppointmentSlotMedicines { get; set; } = new List<AppointmentSlotMedicine>();

    public virtual Patient? Patient { get; set; }

    public virtual Slot? Slot { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
