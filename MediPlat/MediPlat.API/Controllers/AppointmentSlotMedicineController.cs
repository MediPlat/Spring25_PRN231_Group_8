using System;
using System.Linq;
using System.Threading.Tasks;
using MediPlat.Model.Model;
using MediPlat.Model.RequestObject;
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
        [Authorize(Roles = "Doctor,Patient")]
        public IActionResult Get()
        {
            return Ok(_appointmentSlotMedicineService.GetAllAppointmentSlotMedicines());
        }

        [HttpGet("{appointmentSlotId}/{medicineId}")]
        [Authorize(Roles = "Doctor,Patient")]
        public async Task<IActionResult> GetById(Guid appointmentSlotId, Guid medicineId, Guid patientId)
        {
            var result = await _appointmentSlotMedicineService.GetAppointmentSlotMedicineByIdAsync(appointmentSlotId, medicineId, patientId);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Create([FromBody] AppointmentSlotMedicineRequest request)
        {
            var result = await _appointmentSlotMedicineService.AddAppointmentSlotMedicineAsync(request);
            return Created($"odata/AppointmentSlotMedicines/{result.AppointmentSlotMedicineId}", new { result.AppointmentSlotMedicineId });
        }

        [HttpPut("{appointmentSlotId}/{medicineId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Update(Guid appointmentSlotId, Guid medicineId, Guid patientId, [FromBody] AppointmentSlotMedicineRequest request)
        {
            await _appointmentSlotMedicineService.UpdateAppointmentSlotMedicineAsync(appointmentSlotId, medicineId, patientId, request);
            return NoContent();
        }

        [HttpDelete("{appointmentSlotId}/{medicineId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Delete(Guid appointmentSlotId, Guid medicineId, Guid patientId)
        {
            await _appointmentSlotMedicineService.DeleteAppointmentSlotMedicineAsync(appointmentSlotId, medicineId, patientId);
            return NoContent();
        }
    }
}