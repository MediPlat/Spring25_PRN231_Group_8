using MediPlat.Model.Model;
using MediPlat.Model.Schema;
using MediPlat.Model.ResponseObject;
using MediPlat.Service.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Security.Claims;

namespace MediPlat.API.Controllers
{
    [ApiController]
    [Route("odata/Doctors")]

    public class DoctorController : ODataController
    {
        public readonly IDoctorService _service;
        private readonly ILogger<SubscriptionsController> _logger;
        public DoctorController(IDoctorService service, ILogger<SubscriptionsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("profile")]
        [Authorize(Policy = "DoctorPolicy")]
        public async Task<IActionResult> GetDoctorProfile()
        {
            // Truy xuất DoctorId và Role từ các claim
            var doctorId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            _logger.LogDebug($"📌 DoctorId từ Token: {doctorId}");
            var role = User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
            var doctor = await _service.GetByID(Guid.Parse(doctorId));
            if (doctor == null)
            {
                _logger.LogError("❌ Không tìm thấy bác sĩ trong DB.");
                return NotFound();
            }
            return Ok(doctor);
        }

        [HttpPatch("profile/update")]
        [Authorize(Policy = "DoctorPolicy")]
        public IActionResult UpadateProfile([FromBody] DoctorSchema doctor)
        {
            var doctorId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (doctor == null)
            {
                return BadRequest();
            }

            Doctor doc = _service.Update(doctor, doctorId);
            return Ok(doc);

        }


        [HttpPatch("profile/changing_password")]
        [Authorize(Policy = "DoctorPolicy")]

        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword change)
        {
            var doctorId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (change == null)
            {
                return BadRequest();
            }
            bool check = await _service.ChangePassword(change, Guid.Parse(doctorId));
            if (check)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPatch("banned_unbanned")]
        [Authorize(Policy = "AdminPolicy")]

        public async Task<IActionResult> BanDoctor(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            bool check = await _service.Banned(Guid.Parse(id));
            if (check)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("create_doctor")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> CreateDoctor([FromBody] DoctorSchema doctor)
        {
            if(doctor == null)
            {
                return BadRequest();
            }
            Doctor d = await _service.Create(doctor);
            if (d == null)
            {
                return BadRequest();
            }
            return Ok(d);
        }

        [HttpGet("all_doctor")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> GetAllDoctor()
        {
            List<Doctor> doctors = new List<Doctor>();
            doctors = await _service.GetAllDoctor();
            if(doctors.Count > 0)
            {
                return Ok(doctors);
            }
            return BadRequest();
        }

    }
}
