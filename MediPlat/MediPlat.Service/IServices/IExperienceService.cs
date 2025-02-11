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
        IQueryable<ExperienceResponse> GetAllExperiences();
        Task<ExperienceResponse> GetExperienceByIdAsync(Guid id);
        Task<ExperienceResponse> AddExperienceAsync(ExperienceRequest request);
        Task<ExperienceResponse> UpdateExperienceAsync(Guid id, ExperienceRequest request);
        Task DeleteExperienceAsync(Guid id);
    }
}
