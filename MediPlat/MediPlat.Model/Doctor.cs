using System;
using System.Collections.Generic;

namespace MediPlat.Model;

public partial class Doctor
{
    public Guid Id { get; set; }

    public string? UserName { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? AvatarUrl { get; set; }

    public decimal? Balance { get; set; }

    public decimal? FeePerSession { get; set; }

    public string? Degree { get; set; }

    public string? AcademicTitle { get; set; }

    public DateTime? JoinDate { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<DoctorSubcription> DoctorSubcriptions { get; set; } = new List<DoctorSubcription>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
