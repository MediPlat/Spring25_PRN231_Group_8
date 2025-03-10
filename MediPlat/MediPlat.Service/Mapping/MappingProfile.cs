using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.RequestObject.Patient;
using MediPlat.Model.ResponseObject;
using MediPlat.Model.ResponseObject.Patient;
using Profile = AutoMapper.Profile;

namespace MediPlat.Service.Mapping
{
    public class MappingProfile : Profile
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

            CreateMap<AppointmentSlotMedicine, AppointmentSlotMedicineResponse>()
                .ForMember(dest => dest.Medicine, opt => opt.MapFrom(src => src.Medicine ?? new Medicine()));

            CreateMap<AppointmentSlotRequest, AppointmentSlot>()
                .ForMember(dest => dest.Profile, opt => opt.Ignore())
                .ForMember(dest => dest.Slot, opt => opt.Ignore())
                .ForMember(dest => dest.SlotId, opt => opt.MapFrom(src => src.SlotId))
                .ForMember(dest => dest.ProfileId, opt => opt.MapFrom(src => src.ProfileId));

            CreateMap<AppointmentSlot, AppointmentSlotResponse>()
                .ForMember(dest => dest.Doctor, opt => opt.MapFrom(src => src.Slot.Doctor))
                .ForMember(dest => dest.Profile, opt => opt.MapFrom(src => src.Profile))
                .ForMember(dest => dest.Slot, opt => opt.MapFrom(src => src.Slot))
                .ForMember(dest => dest.AppointmentSlotMedicines, opt => opt.MapFrom(src => src.AppointmentSlotMedicines));

            CreateMap<MedicineRequest, Medicine>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ?? "Active"));

            CreateMap<Medicine, MedicineResponse>();

            CreateMap<TransactionRequest, Transaction>();

            CreateMap<Transaction, TransactionResponse>();

            CreateMap<SlotRequest, Slot>();

            CreateMap<Slot, SlotResponse>()
                .ForMember(dest => dest.Doctor, opt => opt.MapFrom(src => src.Doctor))
                .ForMember(dest => dest.Service, opt => opt.MapFrom(src => src.Service));

            CreateMap<ServiceRequest, Model.Model.Service>();

            CreateMap<Model.Model.Service, ServiceResponse>()
                .ForMember(dest => dest.Specialty, opt => opt.MapFrom(src => src.Specialty));

            CreateMap<ReviewRequest, Review>();

            CreateMap<Review, ReviewResponse>();

            CreateMap<PatientRequest, Patient>();

            CreateMap<Patient, PatientResponse>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Profiles.FirstOrDefault().FullName));

            CreateMap<Model.Model.Profile, ProfileRequest>();

            CreateMap<ProfileRequest, Model.Model.Profile>();

            CreateMap<Model.Model.Profile, ProfileResponse>()
                .ForMember(dest => dest.AppointmentSlots, opt => opt.MapFrom(src => src.AppointmentSlots));
        }
    }
}