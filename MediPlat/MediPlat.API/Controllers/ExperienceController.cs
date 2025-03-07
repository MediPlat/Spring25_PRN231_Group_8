using MediPlat.Service.IServices;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace MediPlat.API.Controllers
{
    [Route("odata/Experiences")]
    [ApiController]
    public class ExperienceController : ODataController
    {
        private readonly IExperienceService _experienceService;
        private readonly ILogger<ExperienceController> _logger;

        public ExperienceController(IExperienceService experienceService, ILogger<ExperienceController> logger)
        {
            _experienceService = experienceService;
            _logger = logger;
        }

        [HttpGet]
        [EnableQuery]
        [Authorize(Policy = "DoctorOrAdminorPatientPolicy")]
        public IQueryable<ExperienceResponse> GetExperiences()
        {
            var isPatient = User.FindFirstValue(ClaimTypes.Role) == "Patient";
            return _experienceService.GetAllExperiences(isPatient);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "DoctorOrAdminorPatientPolicy")]
        public async Task<IActionResult> GetExperience(Guid id)
        {
            var isPatient = User.FindFirstValue(ClaimTypes.Role) == "Patient";
            var doctorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var experience = await _experienceService.GetExperienceByIdAsync(id, doctorId, isPatient);
            var result = new
            {
                experience.Id,
                experience.SpecialtyId,
                experience.Title,
                experience.Description,
                experience.Certificate,
                experience.Status,
                experience.DoctorId,
                Doctor = experience.Doctor != null ? new { experience.Doctor.Id, experience.Doctor.FullName } : null,
                Specialty = experience.Specialty != null ? new { experience.Specialty.Id, experience.Specialty.Name } : null
            };

            return Ok(result);

        }

        [HttpPost]
        [Authorize(Policy = "DoctorPolicy")]
        public async Task<IActionResult> CreateExperience([FromBody] ExperienceRequest request)
        {
            var doctorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            request.DoctorId = doctorId;
            var response = await _experienceService.AddExperienceAsync(request);
            return CreatedAtAction(nameof(GetExperience), new { id = response.Id }, response);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "DoctorOrAdminPolicy")]
        public async Task<IActionResult> UpdateExperience(Guid id, [FromBody] ExperienceRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var isAdmin = userRole == "Admin";
            var isPatient = userRole == "Patient";
            var doctorId = Guid.Parse(userId);

            var existingExperience = await _experienceService.GetExperienceByIdAsync(id, doctorId, isPatient);

            if (isAdmin)
            {
                var adminResponse = await _experienceService.UpdateExperienceStatusAsync(id, request.Status);
                return Ok(adminResponse);
            }

            if (existingExperience.DoctorId != doctorId)
            {
                return Forbid();
            }

            var response = await _experienceService.UpdateExperienceWithoutStatusAsync(id, request, doctorId);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DoctorPolicy")]
        public async Task<IActionResult> DeleteExperience(Guid id)
        {
            var doctorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            bool isPatient = User.FindFirstValue(ClaimTypes.Role) == "Patient";

            var existingExperience = await _experienceService.GetExperienceByIdAsync(id, doctorId, isPatient);

            if (existingExperience.DoctorId != doctorId)
            {
                return Forbid();
            }

            await _experienceService.DeleteExperienceAsync(id);
            return NoContent();
        }
    }
}
