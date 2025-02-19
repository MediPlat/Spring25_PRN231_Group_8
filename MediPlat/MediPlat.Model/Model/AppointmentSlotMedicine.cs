using System;
using System.Collections.Generic;

namespace MediPlat.Model.Model;

public partial class AppointmentSlotMedicine
{
    public Guid AppointmentSlotMedicineId { get; set; }

    public Guid? AppointmentSlotId { get; set; }

    public Guid? MedicineId { get; set; }

    public Guid? PatientId { get; set; }

    public string Dosage { get; set; } = null!;

    public string? Instructions { get; set; }

    public short Quantity { get; set; }

    public virtual AppointmentSlot? AppointmentSlot { get; set; }

    public virtual Medicine? Medicine { get; set; }

    public virtual Patient? Patient { get; set; }
}
