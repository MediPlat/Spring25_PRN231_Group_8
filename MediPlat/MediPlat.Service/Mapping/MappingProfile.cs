using AutoMapper;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;

namespace MediPlat.Service.Mapping
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<DoctorSubscriptionRequest, DoctorSubscription>();

            CreateMap<DoctorSubscription, DoctorSubscriptionResponse>();

            CreateMap<SubscriptionRequest, Subscription>();

            CreateMap<Subscription, SubscriptionResponse>();

            CreateMap<ExperienceRequest, Experience>();

            CreateMap<ExperienceRequest, Experience>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            CreateMap<Experience, ExperienceResponse>();

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

            CreateMap<DoctorRequest, Doctor>();

            CreateMap<Doctor, DoctorResponse>();

            CreateMap<Model.Model.Service, ServiceResponse>();

            CreateMap<ServiceRequest, Model.Model.Service>();

            CreateMap<ReviewRequest, Review>();

            CreateMap<Review, ReviewResponse>();

            CreateMap<SpecialtyRequest, Specialty>();

            CreateMap<Specialty, SpecialtyResponse>();
        }
    }
}