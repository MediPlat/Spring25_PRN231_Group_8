using MediPlat.Model;
using MediPlat.Model.Authen_Athor;
using MediPlat.Repository.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MediPlat.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly MediPlatContext _context; // DbContext kết nối tới database
        public AuthController(IConfiguration configuration, MediPlatContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            // Tìm user trong bảng Patient
            var patient = _context.Patients
                .FirstOrDefault(p => p.Email == loginModel.Email);

            if (patient != null)
            {
                // So sánh mật khẩu plain text
                if (patient.Password == loginModel.Password)
                {
                    var token = GenerateJwtToken(patient.Id, "Patient");
                    return Ok(new AuthResult
                    {
                        Token = token,
                        ExpiresAt = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:ExpiresInMinutes"]))
                    });
                }
            }

            // Tìm user trong bảng Doctor
            var doctor = _context.Doctors
                .FirstOrDefault(d => d.Email == loginModel.Email);

            if (doctor != null)
            {
                // So sánh mật khẩu plain text
                if (doctor.Password == loginModel.Password)
                {
                    var token = GenerateJwtToken(doctor.Id, "Doctor");
                    return Ok(new AuthResult
                    {
                        Token = "Bearer " + token,
                        ExpiresAt = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:ExpiresInMinutes"]))
                    });
                }
            }

            return Unauthorized(new { Message = "Invalid credentials" });
        }


        private string GenerateJwtToken(Guid userId, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:ExpiresInMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
