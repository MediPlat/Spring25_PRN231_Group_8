using MediPlat.Model;
using MediPlat.Model.RequestObject.Auth;
using MediPlat.Model.ResponseObject.Auth;
using MediPlat.Repository.IRepositories;
using MediPlat.Service.IServices;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace MediPlat.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IOTPService _oTPService;
        private readonly IPatientRepository _patientRepository;

        public AuthService(ITokenService tokenService, IEmailService emailService, IOTPService oTPService, IPatientRepository patientRepository)
        {
            _tokenService = tokenService;
            _emailService = emailService;
            _oTPService = oTPService;
            _patientRepository = patientRepository;
        }

        public async Task<AuthTokensResponse?> Login(LoginRequest request)
        {
            var patient = await _patientRepository.GetAsync(a => a.Email == request.Email
            && a.Password == HashPassword(request.Password));

            if (patient is null)
            {
                throw new UnauthorizedAccessException("Wrong email or password");
            }

            string accessToken = _tokenService.GenerateAccessToken(patient.Id.ToString(), "Patient", await _oTPService.SendOTPByMail(patient.Email));

            return new AuthTokensResponse
            {
                AccessToken = accessToken,
                Role = "Patient"
            };
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            // Convert the password string to bytes
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Compute the hash
            byte[] hashBytes = sha256.ComputeHash(passwordBytes);

            // Convert the hash to a hexadecimal string
            string hashedPassword = string.Concat(hashBytes.Select(b => $"{b:x2}"));

            return hashedPassword;
        }

        public async Task RegisterAccount(RegisterRequest request)
        {
            var account = await _patientRepository.GetAsync(a => a.Email == request.Email);

            if (account is not null)
            {
                throw new Exception("Account with this email already exists");
            }

            var Id = new Guid();

            var newAccount = new Patient
            {
                Id = Id,
                Email = request.Email,
                Password = HashPassword(request.Password),
            };
            _patientRepository.Add(newAccount);
        }
    }
}
