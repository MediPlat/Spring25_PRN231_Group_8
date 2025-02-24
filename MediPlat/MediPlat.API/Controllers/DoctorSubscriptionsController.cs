using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Service.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
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
            var doctorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return _doctorSubscriptionService.GetAllDoctorSubscriptions();
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "DoctorOrAdminPolicy")]
        public async Task<IActionResult> GetDoctorSubscriptionById(Guid id)
        {
            var doctorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(await _doctorSubscriptionService.GetDoctorSubscriptionByIdAsync(id, doctorId));
        }

        [HttpPost]
        [Authorize(Policy = "DoctorPolicy")]
        public async Task<IActionResult> CreateDoctorSubscription([FromBody] DoctorSubscriptionRequest request)
        {

            if (string.IsNullOrEmpty(request.Status))
            {
                request.Status = "Active";
            }

            var doctorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            return Ok(await _doctorSubscriptionService.AddDoctorSubscriptionAsync(request, doctorId));
        }
        [HttpPut("{id}")]
        [Authorize(Policy = "DoctorPolicy")]
        public async Task<IActionResult> UpdateDoctorSubscription(Guid id, [FromBody] DoctorSubscriptionRequest request)
        {
            var doctorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
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
