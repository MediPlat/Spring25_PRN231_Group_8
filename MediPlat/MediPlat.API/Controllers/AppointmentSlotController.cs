using MediPlat.Model.RequestObject;
using MediPlat.Service.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace MediPlat.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AppointmentSlotController : ControllerBase
    {
        private readonly IAppointmentSlotService _appointmentSlotService;
        private readonly ILogger<AppointmentSlotController> _logger;
        public AppointmentSlotController(IAppointmentSlotService appointmentSlotService, ILogger<AppointmentSlotController> logger)
        {
            _appointmentSlotService = appointmentSlotService;
            _logger = logger;
        }
        [HttpGet]
        [EnableQuery]
        public IActionResult GetAll()
        {
            try
            {
                var result = _appointmentSlotService.GetAppointmentSlot();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAppointmentSlot");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in GetAppointmentSlot");
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentSlotById(Guid id)
        {
            var result = await _appointmentSlotService.GetAppointmentSlotByID(id);
            return result != null ? Ok(result) : NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> CreateAppointmentSlot([FromBody] AppointmentSlotRequest appointmentSlotRequest)
        {
            try
            {
                await _appointmentSlotService.CreateAppointmentSlot(appointmentSlotRequest);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateAppointmentSlot");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in CreateAppointmentSlot");
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAppointmentSlot([FromBody] AppointmentSlotRequest appointmentSlotRequest)
        {
            try
            {
                _appointmentSlotService.UpdateAppointmentSlot(appointmentSlotRequest);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateAppointmentSlot");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in UpdateAppointmentSlot");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointmentSlot(Guid id)
        {
            try
            {
                _appointmentSlotService.DeleteAppointmentSlot(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteAppointmentSlot");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in DeleteAppointmentSlot");
            }
        }
        [HttpGet("{slotId}")]
        public async Task<IActionResult> GetAppointmentSlotBySlotID(Guid slotId)
        {
            var result = await _appointmentSlotService.GetAppointmentSlotBySlotID(slotId);
            return result != null ? Ok(result) : NotFound();
        }
        [HttpGet("{profileId}")]
        public async Task<IActionResult> GetAppointmentSlotByProfileID(Guid profileId)
        {
            var result = await _appointmentSlotService.GetAppointmentSlotByProfileID(profileId);
            return result != null ? Ok(result) : NotFound();
        }
    }
}
