﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MediPlat.Model.ResponseObject
{
    public class SpecialtyResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [JsonIgnore]
        public virtual List<ExperienceResponse> Experiences { get; set; } = new List<ExperienceResponse>();

        [JsonIgnore]
        public virtual List<ServiceResponse> Services { get; set; } = new List<ServiceResponse>();
    }
}
