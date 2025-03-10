using MediPlat.Model.RequestObject;
using MediPlat.Service.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace MediPlat.API.Controllers
{
    [Route("odata/Slots")]
    [ApiController]
    public class SlotController : ODataController
    {
        private readonly ISlotService _slotService;
        private readonly ILogger<SlotController> _logger;
        public SlotController(ISlotService slotService, ILogger<SlotController> logger)
        {
            _slotService = slotService;
            _logger = logger;
        }

        [HttpGet]
        [EnableQuery]
        public IActionResult GetAll() {
            try
            {
                var result = _slotService.GetAllSlot();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllSlot");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in GetAllSlot");
            }
        }

        [HttpGet("{id}")]
        //[Authorize(Roles = "Admin, Patient")]
        public async Task<IActionResult> GetSlotById(Guid id) {
            var result = await _slotService.GetSlotByID(id);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSlot([FromBody] SlotRequest slotRequest)
        {
            try
            {
                _slotService.CreateSlot(slotRequest);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateSlot");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in CreateSlot");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSlot([FromBody] SlotRequest slotRequest)
        {
            try
            {
                _slotService.UpdateSlot(slotRequest);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateSlot");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in UpdateSlot");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSlot(Guid id)
        {
            try
            {
                _slotService.DeleteSlot(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteSlot");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in DeleteSlot");
            }
        }

        [HttpGet("{doctorId}")]
        public async Task<IActionResult> GetSlotByDoctorID(Guid doctorId)
        {
            var result = await _slotService.GetSlotByDoctorID(doctorId);
            return result != null ? Ok(result) : NotFound();
        }
        [HttpGet("{serviceId}")]
        public async Task<IActionResult> GetSlotByServiceID(Guid doctorId)
        {
            var result = await _slotService.GetSlotByDoctorID(doctorId);
            return result != null ? Ok(result) : NotFound();
        }
    }
}
