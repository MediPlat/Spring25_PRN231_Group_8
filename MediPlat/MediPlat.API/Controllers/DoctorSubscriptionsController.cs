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
    [Route("odata/DoctorSubscriptions")]
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
        [Authorize(Policy = "DoctorOrAdminPolicy")]
        public IQueryable<DoctorSubscriptionResponse> GetDoctorSubscriptions()
        {
            return _doctorSubscriptionService.GetAllDoctorSubscriptions();
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "DoctorOrAdminPolicy")]
        public async Task<IActionResult> GetDoctorSubscriptionById(Guid id)
        {
            var doctorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var doctorSubscription = await _doctorSubscriptionService.GetDoctorSubscriptionByIdAsync(id, doctorId);

            if (doctorSubscription == null)
            {
                return NotFound($"Doctor subscription với ID {id} không tồn tại.");
            }

            var result = new
            {
                doctorSubscription.Id,
                doctorSubscription.SubscriptionId,
                doctorSubscription.EnableSlot,
                doctorSubscription.StartDate,
                doctorSubscription.EndDate,
                doctorSubscription.UpdateDate,
                doctorSubscription.Status,
                doctorSubscription.DoctorId,
                Doctor = doctorSubscription.Doctor != null
                    ? new { doctorSubscription.Doctor.Id, doctorSubscription.Doctor.FullName }
                    : null,
                Subscription = doctorSubscription.Subscription != null
                    ? new { doctorSubscription.Subscription.Id, doctorSubscription.Subscription.Name }
                    : null
            };

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "DoctorPolicy")]
        public async Task<IActionResult> CreateDoctorSubscription([FromBody] DoctorSubscriptionRequest request)
        {
            if (string.IsNullOrEmpty(request.Status))
            {
                request.Status = "Active";
            }

            var doctorIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(doctorIdClaim) || !Guid.TryParse(doctorIdClaim, out var doctorId))
            {
                return Unauthorized("Doctor ID không hợp lệ.");
            }
            var newSubscription = await _doctorSubscriptionService.AddDoctorSubscriptionAsync(request, doctorId);
            return Ok(newSubscription);

        }

        [HttpPut("{id}")]
        [Authorize(Policy = "DoctorPolicy")]
        public async Task<IActionResult> UpdateDoctorSubscription(Guid id, [FromBody] DoctorSubscriptionRequest request)
        {
            var doctorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            _logger.LogInformation("Doctor ID from token: {DoctorId}", doctorId);

            var result = await _doctorSubscriptionService.UpdateDoctorSubscriptionAsync(id, request, doctorId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DoctorPolicy")]
        public async Task<IActionResult> DeleteDoctorSubscription(Guid id)
        {
            try
            {
                var doctorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                bool isDeleted = await _doctorSubscriptionService.DeleteDoctorSubscriptionAsync(id, doctorId);

                if (!isDeleted)
                {
                    return NotFound(new { message = "Subscription not found or unauthorized." });
                }

                return Ok(new { message = "Doctor Subscription deleted successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the subscription.", error = ex.Message });
            }
        }
    }
}
