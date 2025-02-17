using System;
using System.Collections.Generic;

namespace MediPlat.Model.Model;

public partial class Medicine
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? DosageForm { get; set; }

    public string Strength { get; set; } = null!;

    public string? SideEffects { get; set; }

    public virtual ICollection<AppointmentSlotMedicine> AppointmentSlotMedicines { get; set; } = new List<AppointmentSlotMedicine>();
}
