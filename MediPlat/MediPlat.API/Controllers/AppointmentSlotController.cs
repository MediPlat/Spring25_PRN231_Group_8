using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
using MediPlat.Model.ResponseObject;
using MediPlat.Service.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace MediPlat.API.Controllers
{
    [ApiController]
    [Route("odata/AppointmentSlots")]
    public class AppointmentSlotController : ODataController
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
        [Authorize(Policy = "DoctorOrAdminOrPatientPolicy")]
        public IQueryable<AppointmentSlotResponse> GetAllAppointmentSlots()
        {
            return _appointmentSlotService.GetAppointmentSlot();
        }

        [HttpGet("doctor/{doctorId}")]
        [Authorize(Policy = "DoctorPolicy")]
        public async Task<IActionResult> GetDoctorAppointments(Guid doctorId)
        {
            var result = await _appointmentSlotService.GetAppointmentSlotsForDoctorAsync(doctorId);
            return Ok(new { value = result });
        }

        [HttpGet("patient/{profileId}")]
        [Authorize(Policy = "PatientPolicy")]
        public async Task<IActionResult> GetProfileAppointments(Guid profileId)
        {
            var result = await _appointmentSlotService.GetAppointmentSlotsForPatientAsync(profileId);

            if (result == null || !result.Any())
            {
                _logger.LogWarning($"⚠ Không có đơn thuốc nào cho profileId: {profileId}");
            }
            else
            {
                _logger.LogInformation($"🟢 Tìm thấy {result.Count} đơn thuốc cho profileId: {profileId}");
            }

            return Ok(new { value = result });
        }

        [HttpGet("doctor/{doctorId}/slot/{appointmentSlotId}")]
        [Authorize(Policy = "DoctorPolicy")]
        public async Task<IActionResult> GetAppointmentSlotByIdForDoctor(Guid doctorId, Guid appointmentSlotId)
        {
            var result = await _appointmentSlotService.GetAppointmentSlotByIdForDoctorAsync(doctorId, appointmentSlotId);

            if (result != null && result.AppointmentSlotMedicines == null)
            {
                result.AppointmentSlotMedicines = new List<AppointmentSlotMedicineResponse>();
            }

            return result != null ? Ok(result) : NotFound();
        }


        [HttpGet("patient/{profileId}/slot/{appointmentSlotId}")]
        [Authorize(Policy = "PatientPolicy")]
        public async Task<IActionResult> GetAppointmentSlotByIdForPatient(Guid profileId, Guid appointmentSlotId)
        {
            var result = await _appointmentSlotService.GetAppointmentSlotByIdForPatientAsync(profileId, appointmentSlotId);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost]
        [Authorize(Policy = "DoctorPolicy")]
        public async Task<IActionResult> Create([FromBody] AppointmentSlotRequest request)
        {
            var result = await _appointmentSlotService.CreateAppointmentSlot(request);
            return result != null ? Created($"odata/AppointmentSlots/{result.Id}", result) : BadRequest("Không thể tạo đơn thuốc.");
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "DoctorPolicy")]
        public async Task<IActionResult> Update(Guid id, [FromBody] AppointmentSlotRequest request)
        {
            var doctorId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (doctorId == null)
            {
                return Unauthorized("Không có quyền cập nhật đơn thuốc.");
            }

            var existingSlot = await _appointmentSlotService.GetAppointmentSlotByIdForDoctorAsync(Guid.Parse(doctorId), id);
            if (existingSlot == null)
            {
                return NotFound();
            }

            if (existingSlot.Status == "Confirmed")
            {
                return BadRequest("Không thể cập nhật trạng thái vì đơn thuốc đã được xác nhận.");
            }

            var result = await _appointmentSlotService.UpdateAppointmentSlot(id, request);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPut("{id}/status")]
        [Authorize(Policy = "DoctorPolicy")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] AppointmentSlotRequest request)
        {
            var doctorId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (doctorId == null)
            {
                return Unauthorized("Không có quyền cập nhật trạng thái.");
            }

            var success = await _appointmentSlotService.UpdateAppointmentSlotStatus(id, request.Status);
            if (!success)
            {
                return BadRequest("Không thể cập nhật trạng thái hoặc đơn thuốc không tồn tại.");
            }

            return NoContent();
        }


        [HttpDelete("{id}")]
        [Authorize(Policy = "DoctorPolicy")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("❌ API nhận ID rỗng.");
                return BadRequest("ID không hợp lệ.");
            }

            var result = await _appointmentSlotService.DeleteAppointmentSlot(id);

            return result ? NoContent() : NotFound("Không tìm thấy đơn thuốc.");
        }
    }
}