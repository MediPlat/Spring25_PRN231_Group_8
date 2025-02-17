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
    [Authorize(Roles = "Doctor")]
    public class DoctorSubscriptionsController : ODataController
    {
        private readonly IDoctorSubscriptionService _doctorSubscriptionService;

        public DoctorSubscriptionsController(IDoctorSubscriptionService doctorSubscriptionService)
        {
            _doctorSubscriptionService = doctorSubscriptionService;
        }

        [HttpGet]
        [EnableQuery]
        public IQueryable<DoctorSubscriptionResponse> GetDoctorSubscriptions()
        {
            var doctorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return _doctorSubscriptionService.GetAllDoctorSubscriptions(doctorId);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorSubscriptionById(Guid id)
        {
            var doctorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(await _doctorSubscriptionService.GetDoctorSubscriptionByIdAsync(id, doctorId));
        }

        [HttpPost]
        public async Task<IActionResult> CreateDoctorSubscription([FromBody] DoctorSubscriptionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var doctorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(await _doctorSubscriptionService.AddDoctorSubscriptionAsync(request, doctorId));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctorSubscription(Guid id, [FromBody] DoctorSubscriptionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var doctorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(await _doctorSubscriptionService.UpdateDoctorSubscriptionAsync(id, request, doctorId));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctorSubscription(Guid id)
        {
            return Forbid();
        }
    }
}
