using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Service.IService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediPlat.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorSubscriptionsController : ControllerBase
    {
        private readonly IDoctorSubscriptionService _doctorSubscriptionService;

        public DoctorSubscriptionsController(IDoctorSubscriptionService doctorSubscriptionService)
        {
            _doctorSubscriptionService = doctorSubscriptionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorSubscriptionResponse>>> GetDoctorSubscriptions()
        {
            var doctorSubscriptions = await _doctorSubscriptionService.GetAllDoctorSubscriptionsAsync();
            return Ok(doctorSubscriptions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorSubscriptionResponse>> GetDoctorSubscription(Guid id)
        {
            var doctorSubscription = await _doctorSubscriptionService.GetDoctorSubscriptionByIdAsync(id);
            if (doctorSubscription == null)
            {
                return NotFound();
            }
            return Ok(doctorSubscription);
        }

        [HttpPost]
        public async Task<ActionResult<DoctorSubscriptionResponse>> CreateDoctorSubscription([FromBody] DoctorSubscriptionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _doctorSubscriptionService.AddDoctorSubscriptionAsync(request);
                return CreatedAtAction(nameof(GetDoctorSubscription), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the doctor subscription: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctorSubscription(Guid id, [FromBody] DoctorSubscriptionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _doctorSubscriptionService.UpdateDoctorSubscriptionAsync(id, request);
                return Ok(response);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the doctor subscription: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctorSubscription(Guid id)
        {
            try
            {
                await _doctorSubscriptionService.DeleteDoctorSubscriptionAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the doctor subscription: {ex.Message}");
            }
        }
    }
}
