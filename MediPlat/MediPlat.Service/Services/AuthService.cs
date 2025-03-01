using MediPlat.Model.Authen_Athor;
using MediPlat.Service.IServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediPlat.Repository.Repositories;
using Microsoft.Extensions.Configuration;
using MediPlat.Repository.IRepositories;
using MediPlat.Model;
using MediPlat.Model.Model;
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

            var admin = _configuration.GetSection("Admins").Get<List<LoginModel>>()
                .FirstOrDefault(a => a.Email == loginModel.Email);

            if (admin != null)
            {
                Console.WriteLine($"Admin found: {admin.Email}");
                if (admin.Password == loginModel.Password)
                {
                    Console.WriteLine("Admin password is correct.");
                    var token = GenerateJwtToken(Guid.NewGuid(), "Admin");
                    result = new AuthResult
                    {
                        Token = "Bearer " + token,
                        ExpiresAt = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:ExpiresInMinutes"]))
                    };
                    return result;
                }
                else
                {
                    Console.WriteLine("Invalid admin password.");
                }
            }
                var patient = _unitOfWork.Patients.Get(c => c.Email == loginModel.Email);
                
            if (patient != null && patient.Status.Equals("Active"))
            {
                Console.WriteLine($"Patient found: {patient.Email}");
                if (patient.Password == loginModel.Password)
                {
                    Console.WriteLine("Patient password is correct.");
                    var token = GenerateJwtToken(patient.Id, "Patient");
                    result = new AuthResult
                    {
                        Token = "Bearer " + token,
                        ExpiresAt = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:ExpiresInMinutes"]))
                    };
                    return result;
                }
                else
                {
                    Console.WriteLine("Invalid patient password.");
                }
            }
            else
            {
                Console.WriteLine("Patient not found.");
            }

            var doctor = _unitOfWork.Doctors.Get(d => d.Email == loginModel.Email);

            if (doctor != null && doctor.Status.Equals("Active"))
            {
                Console.WriteLine($"Doctor found: {doctor.Email}");
                if (doctor.Password == loginModel.Password)
                {
                    Console.WriteLine("Doctor password is correct.");
                    var token = GenerateJwtToken(doctor.Id, "Doctor");
                    result = new AuthResult
                    {
                        Token = "Bearer " + token,
                        ExpiresAt = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:ExpiresInMinutes"]))
                    };
                    return result;
                }
                else
                {
                    Console.WriteLine("Invalid doctor password.");
                }
            }
            else
            {
                Console.WriteLine("Doctor not found.");
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

            Console.WriteLine("Generated Token: " + new JwtSecurityTokenHandler().WriteToken(token));

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
                FullName = registerModel.FullName,
                Email = registerModel.Email,
                Password = registerModel.Password,
                PhoneNumber = registerModel.PhoneNumber,
                JoinDate = DateTime.UtcNow,
                Sex = registerModel.Sex,
                Address = registerModel.Address,
                Status = "Active"
            });

            await _unitOfWork.SaveChangesAsync();
        }


    }
}
