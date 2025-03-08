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
        [Authorize(Policy = "DoctorOrAdminOrPatientPolicy")]
        public IQueryable<AppointmentSlotMedicineResponse> GetAllAppointmentSlotMedicines()
        {
            return _appointmentSlotMedicineService.GetAllAppointmentSlotMedicines();
        }

        [HttpGet("{appointmentSlotId}/{medicineId}")]
        [Authorize(Policy = "DoctorOrAdminOrPatientPolicy")]
        public async Task<IActionResult> GetAppointmentSlotMedicinesById(Guid appointmentSlotId, Guid medicineId)
        {
            var result = await _appointmentSlotMedicineService.GetAppointmentSlotMedicineByIdAsync(appointmentSlotId, medicineId);
            return result != null ? Ok(new { value = result }) : NotFound();
        }

        [HttpPost]
        [Authorize(Policy = "DoctorPolicy")]
        public async Task<IActionResult> CreateAppointmentSlotMedicines([FromBody] AppointmentSlotMedicineRequest request)
        {
            var result = await _appointmentSlotMedicineService.AddAppointmentSlotMedicineAsync(request);
            return Created($"odata/AppointmentSlotMedicines/{result.AppointmentSlotMedicineId}", new { result.AppointmentSlotMedicineId });
        }

        [HttpPut("{appointmentSlotId}/{medicineId}")]
        [Authorize(Policy = "DoctorPolicy")]
        public async Task<IActionResult> UpdateAppointmentSlotMedicines(Guid appointmentSlotId, Guid medicineId, [FromBody] AppointmentSlotMedicineRequest request)
        {
            var result = await _appointmentSlotMedicineService.UpdateAppointmentSlotMedicineAsync(appointmentSlotId, medicineId, request);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpDelete("{appointmentSlotId}/{medicineId}")]
        [Authorize(Policy = "DoctorPolicy")]
        public async Task<IActionResult> DeleteAppointmentSlotMedicines(Guid appointmentSlotId, Guid medicineId)
        {
            await _appointmentSlotMedicineService.DeleteAppointmentSlotMedicineAsync(appointmentSlotId, medicineId);
            return NoContent();
        }
    }
}