using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MediPlat.API.Controllers
{
    [ApiController]
    [Route("api/doctor")]
    [Authorize(Policy = "DoctorPolicy")]
    public class DoctorController : ControllerBase
    {
        [HttpGet("profile")]
        public IActionResult GetDoctorProfile()
        {
            // Truy xuất DoctorId và Role từ các claim
            var doctorId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            var role = User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

            // In ra thông tin về doctorId và role
            Console.WriteLine($"DoctorId: {doctorId}, Role: {role}");

            return Ok(new { Message = "Doctor profile", DoctorId = doctorId });
        }
    }
}
