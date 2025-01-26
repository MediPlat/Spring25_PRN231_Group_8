using MediPlat.Model;
using MediPlat.Model.Authen_Athor;
using MediPlat.Repository.Entities;
using MediPlat.Service.IServices;
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
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            AuthResult login = new AuthResult();
            if (loginModel == null)
            {
                throw new ArgumentNullException(nameof(loginModel));
            }

            login = _authService.Login(loginModel);
            if (login == null)
            {
                // Log thêm chi tiết nếu cần thiết.
                Console.WriteLine("Login response is null");
                return Unauthorized(new { Message = "Invalid credentials" });
            }

            return Ok(login);
        }
    }
}
