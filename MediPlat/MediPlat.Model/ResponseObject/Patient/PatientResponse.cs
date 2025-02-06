namespace MediPlat.Model.ResponseObject.Patient
{
    public class PatientResponse
    {
        public Guid Id { get; set; }

        public string? UserName { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? PhoneNumber { get; set; }

        public decimal? Balance { get; set; }

        public DateTime? JoinDate { get; set; }

        public string? Sex { get; set; }

        public string? Address { get; set; }

        public string? Status { get; set; }
    }
}
