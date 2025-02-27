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
        public IQueryable<ExperienceResponse> GetAllExperiences(bool isPatient)
        {
            var experiences = _unitOfWork.Experiences.GetAll(e => e.Doctor, e => e.Specialty);

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

        public async Task<ExperienceResponse> GetExperienceByIdAsync(Guid id, bool isPatient)
        {
            var experience = await _unitOfWork.Experiences.GetAsync(
                e => e.Id == id,
                e => e.Doctor, e => e.Specialty
            );

            if (experience == null)
                throw new KeyNotFoundException("Experience không tồn tại.");

            if (isPatient && experience.Status != "Active")
            {
                throw new UnauthorizedAccessException("Bạn không có quyền xem Experience này.");
            }

            return _mapper.Map<ExperienceResponse>(experience);
        }

        public async Task<ExperienceResponse> AddExperienceAsync(ExperienceRequest request)
        {
            var existingExperience = await _unitOfWork.Experiences
                .GetAsync(e => e.DoctorId == request.DoctorId && e.SpecialtyId == request.SpecialtyId);

            if (existingExperience != null)
            {
                throw new InvalidOperationException("Bác sĩ đã có Experience với chuyên khoa này.");
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
            var existingExperience = await _unitOfWork.Experiences.GetAsync(
                e => e.Id == id && e.DoctorId == doctorId && e.SpecialtyId == request.SpecialtyId,
                e => e.Doctor, e => e.Specialty
            );

            if (existingExperience == null)
                throw new KeyNotFoundException("Experience không tồn tại hoặc không thuộc về bác sĩ này.");

            _mapper.Map(request, existingExperience);
            request.Status = existingExperience.Status;

            await _unitOfWork.Experiences.UpdatePartialAsync(
                existingExperience,
                e => e.Title,
                e => e.Description,
                e => e.Certificate,
                e => e.SpecialtyId
            );

            return _mapper.Map<ExperienceResponse>(existingExperience);
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