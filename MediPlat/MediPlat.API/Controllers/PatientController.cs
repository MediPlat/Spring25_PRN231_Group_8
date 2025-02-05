using MediPlat.Model.RequestObject.Patient;
using MediPlat.Model.ResponseObject.Patient;
using MediPlat.Service.IServices;
using MediPlat.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;

namespace MediPlat.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<PatientResponse>>> GetAll()
        {
            return await _patientService.GetAll(HttpContext.User);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientResponse>> GetById(string id)
        {
            return await _patientService.GetById(id);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<PatientResponse>> Create([FromForm] PatientRequest patientModel)
        {
            return await _patientService.Create(patientModel, HttpContext.User);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<PatientResponse>> Update(string id, [FromBody] PatientRequest patientModel)
        {
            return await _patientService.Update(id, patientModel, HttpContext.User);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<PatientResponse>> Delete(string id)
        {
            return await _patientService.DeleteById(id);
        }
    }
}
