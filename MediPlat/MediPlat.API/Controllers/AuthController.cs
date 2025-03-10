﻿using AutoMapper;
using MediPlat.Model.Authen_Athor;
using MediPlat.Model.Authen_Author;
using MediPlat.Model.RequestObject.Auth;
using MediPlat.Service.IServices;
using Microsoft.AspNetCore.Mvc;

namespace MediPlat.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        public AuthController(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            if (loginModel == null)
            {
                throw new ArgumentNullException(nameof(loginModel));
            }

            var login = _authService.Login(loginModel);
            if (string.IsNullOrEmpty(login.Token))
            {
                return Unauthorized(new { Message = "Invalid credentials" });
            }

            return Ok(login);
        }

       /* [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            await _authService.RegisterAsync(_mapper.Map<RegisterModel>(registerRequest));
            return Created(nameof(AuthController), new
            {
                StatusCode = 201,
                Message = "Register successful!"
            });
        }*/
    }
}
