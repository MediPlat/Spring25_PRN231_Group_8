using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Service.IServices;
using MediPlat.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace MediPlat.API.Controllers
{
    [Route("odata/Experiences")]
    [ApiController]
    public class ExperienceController : ODataController
    {
        private readonly IExperienceService _experienceService;
        private readonly ILogger<ExperienceService> _logger;
        public ExperienceController(IExperienceService experienceService, ILogger<ExperienceService> logger)
        {
            _experienceService = experienceService;
            _logger = logger;
        }

        [HttpGet]
        [EnableQuery]
        [Authorize(Policy = "DoctorOrAdminPolicy")]
        public IQueryable<ExperienceResponse> GetExperiences()
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            bool isPatient = userRole == "Patient";

            return _experienceService.GetAllExperiences(isPatient);
        }

        [HttpGet("{id}")]
        [EnableQuery]
        [Authorize(Policy = "DoctorOrAdminorPatientPolicy")]
        public IActionResult GetExperience(Guid id)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            bool isPatient = userRole == "Patient";
            var query = _experienceService.GetExperienceByIdQueryable(id, isPatient);

            return Ok(query);
        }

        [HttpPost]
        [Authorize(Policy = "DoctorPolicy")]
        public async Task<IActionResult> CreateExperience([FromBody] ExperienceRequest request)
        {
            try
            {
                var doctorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                request.DoctorId = doctorId;
                var response = await _experienceService.AddExperienceAsync(request);
                return CreatedAtAction(nameof(GetExperience), new { id = response.Id }, response);
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning($"Lỗi khi tạo Experience: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi hệ thống: {ex}");
                return StatusCode(500, "Có lỗi xảy ra khi tạo Experience. Vui lòng thử lại.");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "DoctorOrAdminPolicy")]
        public async Task<IActionResult> UpdateExperience(Guid id, [FromBody] ExperienceRequest request, bool isPatient)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var isAdmin = userRole == "Admin";

            var existingExperience = await _experienceService.GetExperienceByIdQueryable(id, isPatient)
                .FirstOrDefaultAsync();

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

            var existingExperience = await _experienceService.GetExperienceByIdQueryable(id, inPatient)
                .FirstOrDefaultAsync();

            if (existingExperience == null || existingExperience.DoctorId != doctorId)
            {
                return Forbid();
            }

            await _experienceService.DeleteExperienceAsync(id);
            return NoContent();
        }
    }
}
