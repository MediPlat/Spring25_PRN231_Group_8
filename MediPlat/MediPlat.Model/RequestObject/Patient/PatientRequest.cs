namespace MediPlat.Model.RequestObject.Patient
{
        public class PatientRequest
        {
                public string? UserName { get; set; }

                public string? Email { get; set; }

                public string? Password { get; set; }

                public decimal? Balance { get; set; }

                public string? Status { get; set; }
        }
}
