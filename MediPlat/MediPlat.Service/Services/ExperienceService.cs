using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IServices;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            var experiences = _unitOfWork.Experiences.GetAll(e => e.Doctor, e => e.Specialty).Where(e => e.Doctor != null); ;

            if (isPatient)
            {
                experiences = experiences.Where(e => e.Status == "Active");
            }

            return experiences
                .Select(exp => new ExperienceResponse
                {
                    Id = exp.Id,
                    Title = exp.Title,
                    Description = exp.Description,
                    Certificate = exp.Certificate,
                    Status = exp.Status,
                    DoctorId = exp.DoctorId,
                    SpecialtyId = exp.SpecialtyId,
                    Doctor = exp.Doctor != null ? new DoctorResponse
                    {
                        Id = exp.Doctor.Id,
                        FullName = exp.Doctor.FullName ?? "Không có thông tin"
                    } : null,
                    Specialty = exp.Specialty != null ? new SpecialtyResponse
                    {
                        Id = exp.Specialty.Id,
                        Name = exp.Specialty.Name ?? "Không có chuyên khoa"
                    } : null
                })
                .AsQueryable();
        }

        public IQueryable<ExperienceResponse> GetExperienceByIdQueryable(Guid id, bool isPatient)
        {
            var query = _unitOfWork.Experiences.GetAll()
                .Where(e => e.Id == id);
            if (isPatient)
            {
                query = query.Where(e => e.Status == "Active");
            }
            return query.Select(e => new ExperienceResponse
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Certificate = e.Certificate,
                Status = e.Status,
                DoctorId = e.DoctorId,
                SpecialtyId = e.SpecialtyId,
                Doctor = e.Doctor != null ? new DoctorResponse
                {
                    Id = e.Doctor.Id,
                    FullName = e.Doctor.FullName
                } : null,
                Specialty = e.Specialty != null ? new SpecialtyResponse
                {
                    Id = e.Specialty.Id,
                    Name = e.Specialty.Name
                } : null
            });
        }

        public async Task<ExperienceResponse> AddExperienceAsync(ExperienceRequest request)
        {
            var existingExperience = await _unitOfWork.Experiences
                .GetAsync(e => e.DoctorId == request.DoctorId && e.SpecialtyId == request.SpecialtyId);

            if (existingExperience != null)
            {
                _logger.LogWarning($"Bác sĩ {request.DoctorId} đã có Experience với chuyên khoa {request.SpecialtyId}.");
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