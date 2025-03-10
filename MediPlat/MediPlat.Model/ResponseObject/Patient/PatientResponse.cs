using MediPlat.Model.Model;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MediPlat.Model.ResponseObject.Patient
{
    public class PatientResponse
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public decimal? Balance { get; set; }
        public string? Status { get; set; }
        public string? FullName => Profiles?.FirstOrDefault()?.FullName;
        [JsonIgnore]
        public virtual ICollection<ProfileResponse> Profiles { get; set; } = new List<ProfileResponse>();
        [JsonIgnore]
        public virtual ICollection<TransactionResponse> Transactions { get; set; } = new List<TransactionResponse>();
    }
}
