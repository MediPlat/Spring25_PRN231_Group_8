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
    [Route("api/patient")]
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

        [Authorize]
        [EnableQuery]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                bool isValid = Guid.TryParse(id, out temp);
                if (!isValid) 
                {
                    return BadRequest("Incorrect GUID format.");
                }
                var result = await _patientService.GetById(id);
                return result == null ? NotFound($"Can not find patient with id: {id}") : Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] PatientRequest patientModel)
        {
            try
            {
                return Ok(await _patientService.Create(patientModel, HttpContext.User));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] PatientRequest patientModel)
        {
            try
            {
                bool isValid = Guid.TryParse(id, out temp);
                if (!isValid)
                {
                    return BadRequest("Incorrect GUID format.");
                }
                var result = await _patientService.Update(id, patientModel, HttpContext.User);
                return result == null ? NotFound($"Can not find patient with id: {id}") : Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin, Patient")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                bool isValid = Guid.TryParse(id, out temp);
                if (!isValid)
                {
                    return BadRequest("Incorrect GUID format.");
                }
                var result = await _patientService.DeleteById(id);
                return result == null ? NotFound($"Can not find patient with id: {id}") : Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
