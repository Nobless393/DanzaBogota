using DanzaBogota.Application.Dto.Auth;
using DanzaBogota.Application.Interface;
using DanzaBogota.Transversal.Common.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DanzaBogota.Services.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthApplication _authApplication;

        public AuthController(IAuthApplication authApplication)
        {
            _authApplication = authApplication;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authApplication.RegisterAsync(registerDto);

            if (!result.Success)
            {
                return BadRequest(ApiResponse<string>.Fail(result.ErrorMessage ?? "No fue posible completar el registro."));
            }

            return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Registro exitoso."));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authApplication.LoginAsync(loginDto);

            if (!result.Success)
            {
                return Unauthorized(ApiResponse<string>.Fail(result.ErrorMessage ?? "Correo o contraseña incorrectos."));
            }

            return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Inicio de sesión exitoso."));
        }
    }
}
