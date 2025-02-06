using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Service.IServices;
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
        public IActionResult GetDoctorSubscriptions([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var doctorIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (doctorIdClaim == null)
                {
                    return Unauthorized("DoctorId claim is missing.");
                }
                var doctorId = Guid.Parse(doctorIdClaim.Value);

                var doctorSubscriptions = _doctorSubscriptionService.GetAllDoctorSubscriptions(doctorId).Skip((page - 1) * pageSize)
                             .Take(pageSize);

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
            var doctorIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (doctorIdClaim == null || string.IsNullOrEmpty(doctorIdClaim.Value))
            {
                return Unauthorized("Doctor ID is missing.");
            }

            if (!Guid.TryParse(doctorIdClaim.Value, out Guid doctorId))
            {
                return Unauthorized("Invalid Doctor ID format.");
            }

            var doctorSubscription = await _doctorSubscriptionService.GetDoctorSubscriptionByIdAsync(id, doctorId);

            if (doctorSubscription == null)
            {
                return NotFound($"Doctor subscription with ID {id} not found.");
            }

            return Ok(doctorSubscription);
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
                var doctorIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (doctorIdClaim == null || string.IsNullOrEmpty(doctorIdClaim.Value) || !Guid.TryParse(doctorIdClaim.Value, out Guid doctorId))
                {
                    return Unauthorized("Doctor ID is missing or invalid.");
                }

                var existingSubscriptions = _doctorSubscriptionService.GetAllDoctorSubscriptions(doctorId);
                if (existingSubscriptions.Any(ds => ds.SubscriptionId == request.SubscriptionId))
                {
                    return Conflict("Doctor already has an active subscription with this SubscriptionId.");
                }

                var startDate = request.StartDate ?? DateTime.Now;
                var endDate = request.EndDate ?? startDate.AddMonths(1);

                var doctorSubscription = new DoctorSubscriptionRequest
                {
                    SubscriptionId = request.SubscriptionId,
                    EnableSlot = request.EnableSlot,
                    DoctorId = doctorId,
                    StartDate = startDate,
                    EndDate = endDate,
                    UpdateDate = request.UpdateDate
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
                var doctorIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (doctorIdClaim == null || string.IsNullOrEmpty(doctorIdClaim.Value) || !Guid.TryParse(doctorIdClaim.Value, out Guid doctorId))
                {
                    return Unauthorized("Doctor ID is missing or invalid.");
                }

                var existingSubscription = await _doctorSubscriptionService.GetDoctorSubscriptionByIdAsync(id, doctorId);
                if (existingSubscription == null)
                {
                    return NotFound($"Doctor subscription with ID {id} not found.");
                }

                var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var updatedSubscription = new DoctorSubscriptionRequest
                {
                    SubscriptionId = existingSubscription.SubscriptionId,
                    EnableSlot = request.EnableSlot,
                    DoctorId = existingSubscription.DoctorId,
                    StartDate = existingSubscription.StartDate,
                    EndDate = existingSubscription.EndDate == DateTime.MinValue
                              ? existingSubscription.StartDate.AddMonths(1)
                              : existingSubscription.EndDate,
                    UpdateDate = TimeZoneInfo.ConvertTime(DateTime.Now, localTimeZone)
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
                var doctorIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (doctorIdClaim == null || string.IsNullOrEmpty(doctorIdClaim.Value) || !Guid.TryParse(doctorIdClaim.Value, out Guid doctorId))
                {
                    return Unauthorized("Doctor ID is missing or invalid.");
                }

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