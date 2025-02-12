using System;
using System.Collections.Generic;

namespace MediPlat.Model;

public partial class DoctorSubscription
{
    public Guid Id { get; set; }

    public Guid? SubscriptionId { get; set; }

    public short? EnableSlot { get; set; }

    public Guid? DoctorId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? Status { get; set; }

    public virtual Doctor? Doctor { get; set; }

    public virtual Subscription? Subscription { get; set; }
}
