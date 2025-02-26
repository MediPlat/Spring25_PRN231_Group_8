﻿using MediPlat.Model.RequestObject;
using MediPlat.Service.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace MediPlat.API.Controllers
{
    [Route("api/[controller]")]
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
        public IActionResult Get() {
            try
            {
                var result = _slotService.GetSlot();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetSlot");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in GetSlot");
            }
        }
        [HttpGet("{id}")]
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
    }
}
