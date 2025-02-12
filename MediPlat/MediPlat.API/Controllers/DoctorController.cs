using MediPlat.Model.Model;
using MediPlat.Model.Schema;
using MediPlat.Service.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MediPlat.API.Controllers
{
    [ApiController]
    [Route("api/doctor")]

    public class DoctorController : ControllerBase
    {
        public readonly IDoctorService _service;
        public DoctorController(IDoctorService service)
        {
            _service = service;
        }

        [HttpGet("profile")]
        [Authorize(Policy = "DoctorPolicy")]
        public async Task<IActionResult> GetDoctorProfile()
        {
            // Truy xuất DoctorId và Role từ các claim
            var doctorId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            var role = User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
            var doctor = await _service.GetByID(Guid.Parse(doctorId));
            if (doctor == null)
            {
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

        [HttpPatch("banned")]
        [Authorize(Policy = "DoctorPolicy")]

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

    }
}
