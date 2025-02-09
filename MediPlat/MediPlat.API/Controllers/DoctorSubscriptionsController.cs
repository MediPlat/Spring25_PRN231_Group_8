using MediPlat.Model.Model;
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
        public IQueryable<DoctorSubscriptionResponse> GetDoctorSubscriptions()
        {
            var doctorIdClaim = User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (doctorIdClaim == null)
            {
                return Enumerable.Empty<DoctorSubscriptionResponse>().AsQueryable();
            }
            var doctorId = Guid.Parse(doctorIdClaim.Value);

            return _doctorSubscriptionService.GetAllDoctorSubscriptions(doctorId);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetDoctorSubscriptions(Guid id)
        {
            var doctorIdClaim = User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
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
            var doctorIdClaim = User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
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

        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdateDoctorSubscription(Guid id, [FromBody] DoctorSubscriptionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var doctorIdClaim = User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (doctorIdClaim == null || string.IsNullOrEmpty(doctorIdClaim.Value) || !Guid.TryParse(doctorIdClaim.Value, out Guid doctorId))
            {
                return Unauthorized("Doctor ID is missing or invalid.");
            }

            var existingSubscription = await _doctorSubscriptionService.GetDoctorSubscriptionByIdAsync(id, doctorId);
            if (existingSubscription == null)
            {
                return NotFound($"Doctor subscription with ID {id} not found.");
            }

            if (existingSubscription.DoctorId != doctorId)
            {
                return Forbid("You do not have permission to modify this subscription.");
            }

            var response = await _doctorSubscriptionService.UpdateDoctorSubscriptionAsync(id, request, doctorId);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> DeleteDoctorSubscription(Guid id)
        {
            var doctorIdClaim = User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (doctorIdClaim == null || string.IsNullOrEmpty(doctorIdClaim.Value) || !Guid.TryParse(doctorIdClaim.Value, out Guid doctorId))
            {
                return Unauthorized("Doctor ID is missing or invalid.");
            }

            var existingSubscription = await _doctorSubscriptionService.GetDoctorSubscriptionByIdAsync(id, doctorId);
            if (existingSubscription == null)
            {
                return NotFound($"Doctor subscription with ID {id} not found.");
            }

            if (existingSubscription.DoctorId != doctorId)
            {
                return Forbid("You do not have permission to delete this subscription.");
            }

            await _doctorSubscriptionService.DeleteDoctorSubscriptionAsync(id, doctorId);
            return NoContent();
        }
    }
}