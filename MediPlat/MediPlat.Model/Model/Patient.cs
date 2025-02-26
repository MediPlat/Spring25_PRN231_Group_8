using System;
using System.Collections.Generic;

namespace MediPlat.Model.Model;

public partial class Patient
{
    public Guid Id { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public decimal? Balance { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Profile> Profiles { get; set; } = new List<Profile>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
