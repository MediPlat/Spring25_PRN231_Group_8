using MediPlat.Model.Authen_Athor;
using MediPlat.Service.IServices;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediPlat.Model;
using Microsoft.Extensions.Configuration;
using MediPlat.Repository.IRepositories;

namespace MediPlat.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IGenericRepository<Doctor> _doctor_repository;
        private readonly IGenericRepository<Patient> _patient_repository;
        public AuthService(IConfiguration configuration, IGenericRepository<Doctor> doctor_repository, IGenericRepository<Patient> patient_repository)
        {
            _configuration = configuration;
            _doctor_repository = doctor_repository;
            _patient_repository = patient_repository;
        }

        public AuthResult Login(LoginModel loginModel)
        {
            AuthResult result = new AuthResult();
            var patient = _patient_repository.Get(c => c.Email == loginModel.Email);

            if (patient != null)
            {
                // So sánh mật khẩu plain text
                if (patient.Password == loginModel.Password)
                {
                    var token = GenerateJwtToken(patient.Id, "Patient");
                    result = new AuthResult
                    {
                        Token = "Bearer " + token,
                        ExpiresAt = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:ExpiresInMinutes"]))
                    };
                    return result;
                }
            }

            var doctor = _doctor_repository.Get(d => d.Email == loginModel.Email);

            if (doctor != null)
            {
                // So sánh mật khẩu plain text
                if (doctor.Password == loginModel.Password)
                {
                    var token = GenerateJwtToken(doctor.Id, "Doctor");
                    result = new AuthResult
                    {
                        Token ="Bearer " + token,
                        ExpiresAt = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:ExpiresInMinutes"]))
                    };
                    return result;
                }
            }
            
            return result;           
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
