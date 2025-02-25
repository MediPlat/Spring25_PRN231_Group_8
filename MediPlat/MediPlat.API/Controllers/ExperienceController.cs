using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Service.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Security.Claims;

namespace MediPlat.API.Controllers
{
    [Route("odata/Experiences")]
    [ApiController]
    public class ExperienceController : ODataController
    {
        private readonly IExperienceService _experienceService;

        public ExperienceController(IExperienceService experienceService)
        {
            _experienceService = experienceService;
        }

        [HttpGet]
        [EnableQuery]
        [Authorize(Policy = "DoctorOrAdminorPatientPolicy")]
        public IQueryable<ExperienceResponse> GetExperiences()
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            bool isPatient = userRole == "Patient";

            return _experienceService.GetAllExperiences(isPatient);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "DoctorOrAdminorPatientPolicy")]
        public async Task<IActionResult> GetExperience(Guid id)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            bool isPatient = userRole == "Patient";

            var experience = await _experienceService.GetExperienceByIdAsync(id, isPatient);
            return experience != null ? Ok(experience) : NotFound($"Experience với ID {id} không tồn tại.");
        }

        [HttpPost]
        [Authorize(Policy = "DoctorPolicy")]
        public async Task<IActionResult> CreateExperience([FromBody] ExperienceRequest request)
        {
            if (request.DoctorId != null && request.DoctorId != Guid.Empty)
            {
                return BadRequest("DoctorId should not be provided. It is assigned automatically.");
            }

            var doctorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            request.DoctorId = doctorId;

            var response = await _experienceService.AddExperienceAsync(request);
            return CreatedAtAction(nameof(GetExperience), new { id = response.Id }, response);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "DoctorOrAdminPolicy")]
        public async Task<IActionResult> UpdateExperience(Guid id, [FromBody] ExperienceRequest request, bool isPatient)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var isAdmin = userRole == "Admin";

            var existingExperience = await _experienceService.GetExperienceByIdAsync(id, isPatient);
            if (existingExperience == null)
            {
                return NotFound("Experience không tồn tại.");
            }

            if (isAdmin)
            {
                var adminResponse = await _experienceService.UpdateExperienceStatusAsync(id, request.Status);
                return Ok(adminResponse);
            }

            if (!Guid.TryParse(userId, out var doctorId))
            {
                return Unauthorized("Invalid doctor ID.");
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
        public async Task<IActionResult> DeleteExperience(Guid id, bool inPatient)
        {
            var doctorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var existingExperience = await _experienceService.GetExperienceByIdAsync(id, inPatient);

            if (existingExperience == null || existingExperience.DoctorId != doctorId)
            {
                return Forbid();
            }

            await _experienceService.DeleteExperienceAsync(id);
            return NoContent();
        }
    }
}
