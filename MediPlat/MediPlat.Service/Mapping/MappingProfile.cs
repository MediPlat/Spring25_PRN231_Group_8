using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.RequestObject.Patient;
using MediPlat.Model.ResponseObject;
using MediPlat.Model.ResponseObject.Patient;
using Profile = AutoMapper.Profile;

namespace MediPlat.Service.Mapping
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<DoctorSubscriptionRequest, DoctorSubscription>();

            CreateMap<DoctorSubscription, DoctorSubscriptionResponse>()
                .ForMember(dest => dest.Doctor, opt => opt.MapFrom(src => src.Doctor))
                .ForMember(dest => dest.Subscription, opt => opt.MapFrom(src => src.Subscription));

            CreateMap<SubscriptionRequest, Subscription>();

            CreateMap<Subscription, SubscriptionResponse>();

            CreateMap<ExperienceRequest, Experience>()
                .ForMember(dest => dest.Doctor, opt => opt.Ignore())
                .ForMember(dest => dest.Specialty, opt => opt.Ignore());

            CreateMap<Experience, ExperienceResponse>()
                .ForMember(dest => dest.Doctor, opt => opt.MapFrom(src => src.Doctor))
                .ForMember(dest => dest.Specialty, opt => opt.MapFrom(src => src.Specialty));

            CreateMap<DoctorRequest, Doctor>();

            CreateMap<Doctor, DoctorResponse>();

            CreateMap<SpecialtyRequest, Specialty>();

            CreateMap<Specialty, SpecialtyResponse>();

            CreateMap<AppointmentSlotMedicineRequest, AppointmentSlotMedicine>();

            CreateMap<AppointmentSlotMedicine, AppointmentSlotMedicineResponse>();

            CreateMap<MedicineRequest, Medicine>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ?? "Active"));

            CreateMap<Medicine, MedicineResponse>();

            CreateMap<TransactionRequest, Transaction>();

            CreateMap<Transaction, TransactionResponse>();

            CreateMap<SlotRequest, Slot>();

            CreateMap<Slot, SlotResponse>();

            CreateMap<AppointmentSlotRequest, AppointmentSlot>();

            CreateMap<AppointmentSlot, AppointmentSlotResponse>();

            CreateMap<Model.Model.Service, ServiceResponse>();

            CreateMap<ServiceRequest, Model.Model.Service>();

            CreateMap<ReviewRequest, Review>();

            CreateMap<Review, ReviewResponse>();

            CreateMap<Patient, PatientRequest>();

            CreateMap<PatientRequest, Patient>();

            CreateMap<Patient, PatientResponse>();

            CreateMap<Model.Model.Profile, ProfileRequest>();

            CreateMap<ProfileRequest, Model.Model.Profile>();

            CreateMap<Model.Model.Profile, ProfileResponse>();
        }
    }
}