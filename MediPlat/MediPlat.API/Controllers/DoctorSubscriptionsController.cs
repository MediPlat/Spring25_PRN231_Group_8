using MediPlat.Model;
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
        public async Task<ActionResult<IEnumerable<DoctorSubscription>>> GetDoctorSubscriptions()
        {
            return Ok(await _doctorSubscriptionService.GetAllDoctorSubscriptionsAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorSubscription>> GetDoctorSubscription(Guid id)
        {
            var doctorSubscription = await _doctorSubscriptionService.GetDoctorSubscriptionByIdAsync(id);
            if (doctorSubscription == null)
            {
                return NotFound();
            }
            return Ok(doctorSubscription);
        }

        [HttpPost]
        public async Task<ActionResult<DoctorSubscription>> CreateDoctorSubscription(DoctorSubscription doctorSubscription)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _doctorSubscriptionService.AddDoctorSubscriptionAsync(doctorSubscription);
                return CreatedAtAction(nameof(GetDoctorSubscription), new { id = doctorSubscription.Id }, doctorSubscription);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the doctor subscription.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctorSubscription(Guid id, DoctorSubscription doctorSubscription)
        {
            if (id != doctorSubscription.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _doctorSubscriptionService.UpdateDoctorSubscriptionAsync(doctorSubscription);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the doctor subscription.");
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
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the doctor subscription.");
            }
        }
    }
}