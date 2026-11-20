using DanzaBogota.Application.Dto.User;
using DanzaBogota.Application.Interface;
using DanzaBogota.Transversal.Common.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DanzaBogota.Services.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserApplication _userApplication;

        public UserController(IUserApplication userApplication)
        {
            _userApplication = userApplication;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _userApplication.GetCurrentUserAsync(userId);

            if (result == null)
            {
                return NotFound(ApiResponse<string>.Fail("Usuario no encontrado."));
            }

            return Ok(ApiResponse<UserDto>.Ok(result));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userApplication.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<UserDto>>.Ok(result));
        }

        [HttpPatch("change-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeRole([FromBody] ChangeRoleDto dto)
        {
            var result = await _userApplication.ChangeRoleAsync(dto);

            if (!result)
            {
                return BadRequest(ApiResponse<string>.Fail("No se pudo cambiar el rol. Verifica el usuario y el rol indicado."));
            }

            return Ok(ApiResponse<string>.Ok("Rol actualizado correctamente."));
        }
    }
}
