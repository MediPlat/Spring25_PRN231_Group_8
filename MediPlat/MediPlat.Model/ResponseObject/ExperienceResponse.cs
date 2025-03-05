namespace MediPlat.Model.ResponseObject
{
    public class ExperienceResponse
    {
        public Guid Id { get; set; }

        public Guid? SpecialtyId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Certificate { get; set; }

        public string? Status { get; set; }

        public Guid? DoctorId { get; set; }

        public DoctorResponse Doctor { get; set; } = null!;

        public SpecialtyResponse Specialty { get; set; } = null!;
    }
}
