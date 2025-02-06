using MediPlat.Model;
using MediPlat.Service.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace MediPlat.API.Controllers
{
    [ApiController]
    [Route("odata/DoctorSubscriptions")]
    public class DoctorSubcriptionController : ODataController
    {
        private readonly IDoctorSupcriptionService _service;
        public DoctorSubcriptionController(IDoctorSupcriptionService service)
        {
            _service = service;
        }

        [HttpGet]
        [EnableQuery]
        [Authorize(Policy = "DoctorPolicy")]
        public async Task<IActionResult> GetAll() 
        {
            var doctorId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            List<DoctorSubscription> doctorSubscriptions = new List<DoctorSubscription>();
            doctorSubscriptions = await _service.GetDoctorSubcriptions(Guid.Parse(doctorId));
            return Ok(doctorSubscriptions);
        }
    }
}
