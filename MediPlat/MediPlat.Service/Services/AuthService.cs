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
using MediPlat.Repository.Entities;
using Microsoft.Extensions.Configuration;
using MediPlat.Repository.IRepositories;
using MediPlat.Model;

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

            var doctor = _doctor_repository.Get(d => d.Email == loginModel.Email);

            if (doctor != null)
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

    }
}
