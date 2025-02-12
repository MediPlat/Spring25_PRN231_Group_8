using AutoMapper;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.RequestObject.Patient;
using MediPlat.Model.ResponseObject;
using MediPlat.Model.ResponseObject.Patient;

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

            CreateMap<PatientRequest, Patient>();

            CreateMap<Patient, PatientResponse>();

        }
    }
}