using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MediPlat.Model.ResponseObject
{
    public class DoctorResponse
    {
        public Guid Id { get; set; }

        public string? UserName { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? AvatarUrl { get; set; }

        public decimal? Balance { get; set; }

        public decimal? FeePerHour { get; set; }

        public string? Degree { get; set; }

        public string? AcademicTitle { get; set; }

        public DateTime? JoinDate { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Status { get; set; }

        [JsonIgnore]
        public virtual ICollection<DoctorSubscriptionResponse> DoctorSubscriptions { get; set; } = new List<DoctorSubscriptionResponse>();

        [JsonIgnore]
        public virtual ICollection<ExperienceResponse> Experiences { get; set; } = new List<ExperienceResponse>();

        [JsonIgnore]
        public virtual ICollection<SlotResponse> Slots { get; set; } = new List<SlotResponse>();
    }
}
