﻿using MediPlat.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Model.ResponseObject
{
    public class ProfileResponse
    {
        public Guid Id { get; set; }

        public Guid? PatientId { get; set; }

        public string? FullName { get; set; }

        public string? Sex { get; set; }

        public DateTime? Dob { get; set; }

        public string? Address { get; set; }

        public DateTime? JoinDate { get; set; }

        public string? PhoneNumber { get; set; }

        public virtual ICollection<AppointmentSlotResponse> AppointmentSlots { get; set; } = new List<AppointmentSlotResponse>();

    }
}
