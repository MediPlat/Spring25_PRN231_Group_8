using MediPlat.Model.RequestObject.Auth;
using MediPlat.Model.RequestObject.Patient;
using MediPlat.Model.ResponseObject.Patient;
using MediPlat.Service.IServices;
using MediPlat.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace MediPlat.API.Controllers
{
    [ApiController]
    [Route("odata/patient")]
    public class PatientController : ODataController
    {
        private readonly IPatientService _patientService;
        static Guid temp;
        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        //[Authorize(Roles = "Admin")]
        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _patientService.GetAll(HttpContext.User));
        }

        [Authorize(Roles = "Admin, Patient")]
        [EnableQuery]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _patientService.GetById(id);
            return Ok(result);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] PatientRequest patientModel)
        {
            return Ok(await _patientService.Create(patientModel, HttpContext.User));
        }

        [Authorize(Roles = "Admin, Patient")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] PatientRequest patientModel)
        {
            var result = await _patientService.Update(patientModel, HttpContext.User);
            return Ok(result);
        }

        [Authorize(Roles = "Admin, Patient")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete()
        {
            var result = await _patientService.DeleteById(HttpContext.User);
            return Ok(result);
        }

        [Authorize(Roles = "Patient")]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordRequest changePasswordRequest)
        {
            var result = await _patientService.ChangePassword(HttpContext.User, changePasswordRequest);
            return Ok(result);
        }
    }
}
