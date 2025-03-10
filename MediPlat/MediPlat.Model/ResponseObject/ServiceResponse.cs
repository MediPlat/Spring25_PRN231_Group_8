using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MediPlat.Model.ResponseObject
{
    public class ServiceResponse
    {
        public Guid Id { get; set; }

        public Guid? SpecialtyId { get; set; }
        public string? SpecialtyName { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public virtual SpecialtyResponse? Specialty { get; set; }

        [JsonIgnore]
        public virtual ICollection<SlotResponse> Slots { get; set; } = new List<SlotResponse>();
    }
}
