using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Service.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.Text.Json;

namespace MediPlat.API.Controllers
{
    [Route("odata/Slots")]
    [ApiController]
    public class SlotController : ControllerBase
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
        [Authorize(Policy = "DoctorOrPatientPolicy")]
        public IQueryable<SlotResponse> Get()
        {
            return _slotService.GetSlot();;
        }

        [Authorize(Policy = "DoctorOrPatientPolicy")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSlotById(Guid id)
        {
            var result = await _slotService.GetSlotByID(id);
            return result != null ? Ok(result) : NotFound();
        }

        [Authorize(Policy = "DoctorPolicy")]
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

        [Authorize(Policy = "DoctorPolicy")]
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

        [Authorize(Policy = "DoctorPolicy")]
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
    }
}
