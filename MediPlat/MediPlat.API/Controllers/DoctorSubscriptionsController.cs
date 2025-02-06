using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Service.IServices;
using MediPlat.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Security.Claims;

namespace MediPlat.API.Controllers
{
    [ApiController]
    [Route("api/doctorsubscription")]
    public class DoctorSubscriptionsController : ODataController
    {
        private readonly IDoctorSubscriptionService _doctorSubscriptionService;
        private readonly ILogger<DoctorSubscriptionsController> _logger;

        public DoctorSubscriptionsController(IDoctorSubscriptionService doctorSubscriptionService, ILogger<DoctorSubscriptionsController> logger)
        {
            _doctorSubscriptionService = doctorSubscriptionService;
            _logger = logger;
        }

        [HttpGet]
        [EnableQuery]
        [Authorize(Roles = "Doctor")]
        public IActionResult GetDoctorSubscriptions()
        {
            try
            {
                var doctorIdClaim = User.Claims.FirstOrDefault(c => c.Type == "DoctorId");
                if (doctorIdClaim == null)
                {
                    return Unauthorized("DoctorId claim is missing.");
                }
                var doctorId = Guid.Parse(doctorIdClaim.Value);

                var doctorSubscriptions = _doctorSubscriptionService.GetAllDoctorSubscriptions(doctorId);

                return Ok(doctorSubscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching doctorSubscriptions");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetDoctorSubscriptions(Guid id)
        {
            try
            {
                var doctorId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "DoctorId").Value);
                var doctorSubscription = await _doctorSubscriptionService.GetDoctorSubscriptionByIdAsync(id, doctorId);

                if (doctorSubscription == null)
                {
                    return NotFound($"Doctor subscription with ID {id} not found.");
                }

                return Ok(doctorSubscription);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Doctor subscription not found: {Id}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching doctor subscription with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CreateDoctorSubscription([FromBody] DoctorSubscriptionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var doctorId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "DoctorId").Value);

                var existingSubscriptions = _doctorSubscriptionService.GetAllDoctorSubscriptions(doctorId);
                if (existingSubscriptions.Any(ds => ds.SubscriptionId == request.SubscriptionId))
                {
                    return Conflict("Doctor already has an active subscription with this SubscriptionId.");
                }

                var startDate = request.StartDate ?? DateTime.Now;
                var endDate = request.EndDate.HasValue ? request.EndDate.Value : startDate.AddMonths(1);

                var doctorSubscription = new DoctorSubscription
                {
                    Id = Guid.NewGuid(),
                    SubscriptionId = request.SubscriptionId,
                    EnableSlot = request.EnableSlot,
                    DoctorId = doctorId,
                    StartDate = startDate,
                    EndDate = endDate,
                    UpdateDate = request.UpdateDate ?? (DateTime?)null
                };

                await _doctorSubscriptionService.AddDoctorSubscriptionAsync(request, doctorId);
                return Ok(doctorSubscription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating doctor subscription");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdateDoctorSubscription(Guid id, [FromBody] DoctorSubscriptionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var doctorId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "DoctorId").Value);
                var existingSubscription = await _doctorSubscriptionService.GetDoctorSubscriptionByIdAsync(id, doctorId);

                if (existingSubscription == null)
                {
                    return NotFound($"Doctor subscription with ID {id} not found.");
                }
                var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var updatedSubscription = new DoctorSubscription
                {
                    Id = existingSubscription.Id,
                    SubscriptionId = existingSubscription.SubscriptionId,
                    EnableSlot = request.EnableSlot,
                    DoctorId = existingSubscription.DoctorId,
                    StartDate = existingSubscription.StartDate,
                    EndDate = existingSubscription.EndDate == DateTime.MinValue ? existingSubscription.StartDate.AddMonths(1) : existingSubscription.EndDate, // ✅ Fix lỗi EndDate null
                    UpdateDate = TimeZoneInfo.ConvertTime(DateTime.UtcNow, localTimeZone)
                };

                var response = await _doctorSubscriptionService.UpdateDoctorSubscriptionAsync(id, updatedSubscription, doctorId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating doctor subscription: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> DeleteDoctorSubscription(Guid id)
        {
            try
            {
                var doctorId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "DoctorId").Value);

                var existingSubscription = await _doctorSubscriptionService.GetDoctorSubscriptionByIdAsync(id, doctorId);
                if (existingSubscription == null)
                {
                    return NotFound($"Doctor subscription with ID {id} not found.");
                }

                await _doctorSubscriptionService.DeleteDoctorSubscriptionAsync(id, doctorId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting doctor subscription: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}