using System;
using System.Collections.Generic;

namespace MediPlat.Model.Model;

public partial class Transaction
{
    public Guid Id { get; set; }

    public Guid? PatientId { get; set; }

    public Guid? DoctorId { get; set; }

    public Guid? SubId { get; set; }

    public Guid? AppointmentSlotId { get; set; }

    public string? TransactionInfo { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? Status { get; set; }

    public virtual AppointmentSlot? AppointmentSlot { get; set; }

    public virtual Doctor? Doctor { get; set; }

    public virtual Patient? Patient { get; set; }

    public virtual Subscription? Sub { get; set; }
}
