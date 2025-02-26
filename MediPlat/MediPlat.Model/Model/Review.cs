using System;
using System.Collections.Generic;

namespace MediPlat.Model.Model;

public partial class Review
{
    public Guid Id { get; set; }

    public Guid? SlotId { get; set; }

    public byte? Rating { get; set; }

    public string? Message { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Slot? Slot { get; set; }
}
