using MediPlat.Service.IServices;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediPlat.Service.Services
{
    public class OTPService : IOTPService
    {
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;


        public OTPService(IEmailService emailService, IConfiguration configuration, ITokenService tokenService)
        {
            _emailService = emailService;
            _configuration = configuration;
            _tokenService = tokenService;
        }

        public async Task<string> SendOTPByMail(string email)
        {
            var chars = "0123456789";
            var random = new Random();
            var result = new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());

            string otp = result.ToString();

            string subject = "OTP from MediPlat";
            string message = $"<p>Here is your OTP:</p> <p>{otp}</p>";

            await _emailService.SendEmailAsync(email, subject, message);

            return otp;
        }
    }
}
