using MediPlat.Model.RequestObject.Patient;
using MediPlat.Model.ResponseObject.Patient;
using MediPlat.Service.IServices;
using MediPlat.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Query;

namespace MediPlat.API.Controllers
{
    [ApiController]
    [Route("api/patient")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [Authorize]
        [EnableQuery]
        [HttpGet]
        public async Task<ActionResult<List<PatientResponse>>> GetAll()
        {
            return await _patientService.GetAll(HttpContext.User);
        }

        [Authorize]
        [EnableQuery]
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientResponse>> GetById(string id)
        {
            return await _patientService.GetById(id);
        }

        [Authorize]
        [EnableQuery]
        [HttpPost]
        public async Task<ActionResult<PatientResponse>> Create([FromForm] PatientRequest patientModel)
        {
            return await _patientService.Create(patientModel, HttpContext.User);
        }

        [Authorize]
        [EnableQuery]
        [HttpPut("{id}")]
        public async Task<ActionResult<PatientResponse>> Update(string id, [FromBody] PatientRequest patientModel)
        {
            return await _patientService.Update(id, patientModel, HttpContext.User);
        }

        [Authorize]
        [EnableQuery]
        [HttpDelete("{id}")]
        public async Task<ActionResult<PatientResponse>> Delete(string id)
        {
            return await _patientService.DeleteById(id);
        }
    }
}
