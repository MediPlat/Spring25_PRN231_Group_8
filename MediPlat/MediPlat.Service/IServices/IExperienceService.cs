using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Service.IServices
{
    public interface IExperienceService
    {
        IQueryable<ExperienceResponse> GetAllExperiences(bool isPatient);
        IQueryable<ExperienceResponse> GetExperienceByIdQueryable(Guid id, bool isPatient);
        Task<ExperienceResponse> AddExperienceAsync(ExperienceRequest request);
        Task DeleteExperienceAsync(Guid id);
        Task<ExperienceResponse> UpdateExperienceStatusAsync(Guid id, string status);
        Task<ExperienceResponse> UpdateExperienceWithoutStatusAsync(Guid id, ExperienceRequest request, Guid doctorId);
    }
}
