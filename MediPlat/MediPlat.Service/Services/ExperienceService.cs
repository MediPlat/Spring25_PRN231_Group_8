using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IServices;
using Microsoft.Extensions.Logging;

namespace MediPlat.Service.Services
{
    public class ExperienceService : IExperienceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ExperienceService> _logger;

        public ExperienceService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ExperienceService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public IQueryable<ExperienceResponse> GetAllExperiences(bool isPatient)
        {
            var experiences = _unitOfWork.Experiences.GetAll(e => e.Doctor, e => e.Specialty);

            if (isPatient)
            {
                experiences = experiences.Where(e => e.Status == "Active");
            }

            var experienceList = experiences.ToList();
            return experienceList.Select(e => _mapper.Map<ExperienceResponse>(e)).AsQueryable();
        }

        public async Task<ExperienceResponse> GetExperienceByIdAsync(Guid id, Guid doctorId, bool isPatient)
        {
            var experience = await _unitOfWork.Experiences
                .GetAll(e => e.Doctor, e => e.Specialty)
                .FirstOrDefaultAsync(e => e.Id == id && (!isPatient || e.Status == "Active"));

            if (experience == null)
            {
                throw new KeyNotFoundException($"Experience with ID {id} not found.");
            }

            return _mapper.Map<ExperienceResponse>(experience);
        }

        public async Task<ExperienceResponse> AddExperienceAsync(ExperienceRequest request)
        {
            var existingExperience = await _unitOfWork.Experiences
                .GetAsync(e => e.DoctorId == request.DoctorId && e.SpecialtyId == request.SpecialtyId);

            if (existingExperience != null)
            {
                throw new ApplicationException("Bác sĩ đã có Experience với chuyên khoa này.");
            }

            var experience = _mapper.Map<Experience>(request);
            experience.Id = Guid.NewGuid();
            experience.Status = "Active";

            _unitOfWork.Experiences.Add(experience);
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
        public async Task<ExperienceResponse> UpdateExperienceWithoutStatusAsync(Guid id, ExperienceRequest request, Guid doctorId)
        {
            var existingExperience = await _unitOfWork.Experiences
                               .GetAll(e => e.Doctor, e => e.Specialty)
                               .FirstOrDefaultAsync(e => e.Id == id && e.DoctorId == doctorId && e.SpecialtyId == request.SpecialtyId);

            if (existingExperience == null)
            {
                throw new KeyNotFoundException("Experience không tồn tại hoặc không thuộc về bác sĩ này.");
            }
            _mapper.Map(request, existingExperience);
            if (request.DoctorId.HasValue)
            {
                existingExperience.DoctorId = request.DoctorId;
            }

            existingExperience.Status ??= "Active";
            await _unitOfWork.Experiences.UpdatePartialAsync(
                existingExperience,
                e => e.Title,
                e => e.Description,
                e => e.Certificate,
                e => e.SpecialtyId,
                e => e.DoctorId,
                e => e.Status
            );

            var response = _mapper.Map<ExperienceResponse>(existingExperience);

            if (response.Doctor != null) response.Doctor.Experiences = null;
            if (response.Specialty != null) response.Specialty.Experiences = null;

            return response;
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