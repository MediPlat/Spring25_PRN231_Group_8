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
    [Route("odata/AppointmentSlotMedicines")]
    public class AppointmentSlotMedicineController : ODataController
    {
        private readonly IAppointmentSlotMedicineService _appointmentSlotMedicineService;

        public AppointmentSlotMedicineController(IAppointmentSlotMedicineService appointmentSlotMedicineService)
        {
            _appointmentSlotMedicineService = appointmentSlotMedicineService;
        }

        [HttpGet]
        [EnableQuery]
        [Authorize(Policy = "DoctorOrPatientPolicy")]
        public IQueryable<AppointmentSlotMedicineResponse> GetAllAppointmentSlotMedicines()
        {
            return _appointmentSlotMedicineService.GetAllAppointmentSlotMedicines();
        }

        [HttpGet("{appointmentSlotId}/{medicineId}")]
        [Authorize(Policy = "DoctorOrPatientPolicy")]
        public async Task<IActionResult> GetById(Guid appointmentSlotId, Guid medicineId, Guid patientId)
        {
            var result = await _appointmentSlotMedicineService.GetAppointmentSlotMedicineByIdAsync(appointmentSlotId, medicineId, patientId);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost]
        [Authorize(Policy = "DoctorPolicy")]
        public async Task<IActionResult> Create([FromBody] AppointmentSlotMedicineRequest request)
        {
            var result = await _appointmentSlotMedicineService.AddAppointmentSlotMedicineAsync(request);
            return Created($"odata/AppointmentSlotMedicines/{result.AppointmentSlotMedicineId}", new { result.AppointmentSlotMedicineId });
        }

        [HttpPut("{appointmentSlotId}/{medicineId}")]
        [Authorize(Policy = "DoctorPolicy")]
        public async Task<IActionResult> Update(Guid appointmentSlotId, Guid medicineId, Guid patientId, [FromBody] AppointmentSlotMedicineRequest request)
        {
            var result = await _appointmentSlotMedicineService.UpdateAppointmentSlotMedicineAsync(appointmentSlotId, medicineId, patientId, request);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpDelete("{appointmentSlotId}/{medicineId}")]
        [Authorize(Policy = "DoctorPolicy")]
        public async Task<IActionResult> Delete(Guid appointmentSlotId, Guid medicineId, Guid patientId)
        {
            await _appointmentSlotMedicineService.DeleteAppointmentSlotMedicineAsync(appointmentSlotId, medicineId, patientId);
            return NoContent();
        }
    }
}