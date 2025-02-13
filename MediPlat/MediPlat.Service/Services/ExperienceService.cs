using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IServices;

namespace MediPlat.Service.Services
{
    public class ExperienceService : IExperienceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExperienceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IQueryable<ExperienceResponse> GetAllExperiences()
        {
            var experiences = _unitOfWork.Experiences.GetAll();
            return experiences.Select(exp => _mapper.Map<ExperienceResponse>(exp)).AsQueryable();
        }

        public async Task<ExperienceResponse> GetExperienceByIdAsync(Guid id)
        {
            var experience = await _unitOfWork.Experiences.GetIdAsync(id);
            if (experience == null)
                throw new KeyNotFoundException("Experience not found.");

            return _mapper.Map<ExperienceResponse>(experience);
        }

        public async Task<ExperienceResponse> AddExperienceAsync(ExperienceRequest request)
        {
            var experience = _mapper.Map<Experience>(request);
            experience.Id = Guid.NewGuid();
            experience.Status = "Active";
            _unitOfWork.Experiences.Add(experience);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ExperienceResponse>(experience);
        }
        public async Task<ExperienceResponse> UpdateExperienceAsync(Guid id, ExperienceRequest request, bool isAdmin)
        {
            var experience = await _unitOfWork.Experiences.GetIdAsync(id);
            if (experience == null)
                throw new KeyNotFoundException("Experience not found.");

            _mapper.Map(request, experience);

            if (!isAdmin)
            {
                request.Status = experience.Status;
            }

            experience.Status = request.Status;

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ExperienceResponse>(experience);
        }

        public async Task<ExperienceResponse> UpdateExperienceStatusAsync(Guid id, string status)
        {
            var experience = await _unitOfWork.Experiences.GetIdAsync(id);
            if (experience == null)
                throw new KeyNotFoundException("Experience không tồn tại.");

            experience.Status = status;
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ExperienceResponse>(experience);
        }

        public async Task<ExperienceResponse> UpdateExperienceWithoutStatusAsync(Guid id, ExperienceRequest request)
        {
            var experience = await _unitOfWork.Experiences.GetIdAsync(id);
            if (experience == null)
                throw new KeyNotFoundException("Experience không tồn tại.");

            _mapper.Map(request, experience);
            request.Status = experience.Status;
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ExperienceResponse>(experience);
        }

        public async Task DeleteExperienceAsync(Guid id)
        {
            var experience = await _unitOfWork.Experiences.GetIdAsync(id);
            if (experience == null)
                throw new KeyNotFoundException("Experience not found.");

            _unitOfWork.Experiences.Remove(experience);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
