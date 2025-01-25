using System;
using System.Collections.Generic;

namespace MediPlat.Model;

public partial class DoctorSubcription
{
    public Guid Id { get; set; }

    public Guid? SubscriptionId { get; set; }

    public byte? EnableSlot { get; set; }

    public string? Description { get; set; }

    public Guid? DoctorId { get; set; }

    public virtual Doctor? Doctor { get; set; }

    public virtual Subscription? Subscription { get; set; }
}
