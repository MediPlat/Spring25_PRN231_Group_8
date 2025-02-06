using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Service.IService;
using MediPlat.Service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace MediPlat.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        [Authorize(Roles = "2")]
        public IActionResult GetSubscriptions()
        {
            try
            {
                var doctorId = Guid.Parse(User.Claims.First(c => c.Type == "DoctorId").Value);
                var doctorSubscriptions = _doctorSubscriptionService.GetAllDoctorSubscriptions(doctorId);
                return Ok(doctorSubscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching doctorSubscriptions");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        [EnableQuery]
        [Authorize(Roles = "2")]
        public IActionResult GetDoctorSubscriptions()
        {
            try
            {
                var doctorId = Guid.Parse(User.Claims.First(c => c.Type == "DoctorId").Value);
                var doctorSubscriptions = _doctorSubscriptionService.GetAllDoctorSubscriptions(doctorId);
                return Ok(doctorSubscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching doctorSubscriptions");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> CreateDoctorSubscription([FromBody] DoctorSubscriptionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var doctorId = Guid.Parse(User.Claims.First(c => c.Type == "DoctorId").Value);
                var response = await _doctorSubscriptionService.AddDoctorSubscriptionAsync(request, doctorId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating doctor subscription");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> UpdateDoctorSubscription(Guid id, [FromBody] DoctorSubscriptionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var doctorId = Guid.Parse(User.Claims.First(c => c.Type == "DoctorId").Value);
                var response = await _doctorSubscriptionService.UpdateDoctorSubscriptionAsync(id, request, doctorId);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Doctor subscription not found: {Id}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating doctor subscription: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> DeleteDoctorSubscription(Guid id)
        {
            try
            {
                var doctorId = Guid.Parse(User.Claims.First(c => c.Type == "DoctorId").Value);
                await _doctorSubscriptionService.DeleteDoctorSubscriptionAsync(id, doctorId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Doctor subscription not found: {Id}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting doctor subscription: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}