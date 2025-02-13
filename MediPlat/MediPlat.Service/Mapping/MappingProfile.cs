using AutoMapper;
using MediPlat.Model;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;

namespace MediPlat.Service.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DoctorSubscriptionRequest, DoctorSubscription>();

            CreateMap<DoctorSubscription, DoctorSubscriptionResponse>();

            CreateMap<SubscriptionRequest, Subscription>();

            CreateMap<Subscription, SubscriptionResponse>();

            CreateMap<ExperienceRequest, Experience>();

            CreateMap<Experience, ExperienceResponse>();

            CreateMap<ExperienceRequest, Experience>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            CreateMap<Experience, ExperienceResponse>();
        }
    }
}