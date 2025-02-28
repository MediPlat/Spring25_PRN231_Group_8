
using MediPlat.Model.Authen_Athor;
using MediPlat.Service.IServices;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediPlat.Model.Model;
using Microsoft.Extensions.Configuration;
using MediPlat.Repository.IRepositories;
using MediPlat.Model.Authen_Author;

namespace MediPlat.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public AuthResult Login(LoginModel loginModel)
        {
            AuthResult result = new AuthResult();
            string userRole = string.Empty;

            var patient = _unitOfWork.Patients.Get(c => c.Email == loginModel.Email);
            if (patient != null && patient.Password == loginModel.Password)
            {
                userRole = "Patient";
            }

            var doctor = _unitOfWork.Doctors.Get(d => d.Email == loginModel.Email);
            if (doctor != null && doctor.Password == loginModel.Password)
            {
                userRole = "Doctor";
            }

            var admin = _configuration.GetSection("Admins").Get<List<LoginModel>>()
                        .FirstOrDefault(a => a.Email == loginModel.Email);

            if (admin != null && admin.Password == loginModel.Password)
            {
                userRole = "Admin";
            }

            if (string.IsNullOrEmpty(userRole))
            {
                return result;
            }

            var token = GenerateJwtToken(Guid.NewGuid(), userRole);

            result = new AuthResult
            {
                Token = "Bearer " + token,
                ExpiresAt = DateTime.Now.AddMinutes(double.Parse(_configuration["JwtSettings:ExpiresInMinutes"])),
                Role = userRole
            };

            return result;
        }


        private string GenerateJwtToken(Guid userId, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["JwtSettings:ExpiresInMinutes"])),
                signingCredentials: creds);

            Console.WriteLine("Generated Token: Bearer " + new JwtSecurityTokenHandler().WriteToken(token));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task RegisterAsync(RegisterModel registerModel)
        {
            var existingPatient = await _unitOfWork.Patients.GetAsync(p => p.Email == registerModel.Email && p.Status.Equals("Active"));
            if (existingPatient != null)
            {
                throw new ArgumentException("Account with this email existed!");
            }

            _unitOfWork.Patients.Add(new Patient
            {
                Id = Guid.NewGuid(),
                UserName = registerModel.UserName,
                Email = registerModel.Email,
                Password = registerModel.Password,
                Status = "Active"
            });

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
