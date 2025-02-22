using MediPlat.Model.RequestObject;
using MediPlat.Service.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        [HttpPost("create")]
        //[Authorize(Policy = "DoctorPolicy")]
        public async Task<IActionResult> CreateSlot([FromBody] SlotRequest slotRequest)
        {
            try
            {
                var result = await _slotService.CreateSlot(slotRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi khi tạo slot: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi khi tạo slot");
            }
        }
    }
}
