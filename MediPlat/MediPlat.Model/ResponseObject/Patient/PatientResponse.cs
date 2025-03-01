using MediPlat.Model.Model;

namespace MediPlat.Model.ResponseObject.Patient
{
    public class PatientResponse
    {
        public Guid Id { get; set; }

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public decimal? Balance { get; set; }

        public string? Status { get; set; }

        public virtual ICollection<ProfileResponse> Profiles { get; set; } = new List<ProfileResponse>();

        public virtual ICollection<TransactionResponse> Transactions { get; set; } = new List<TransactionResponse>();
    }
}
