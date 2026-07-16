using DanzaBogota.Application.Dto.User;
using DanzaBogota.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DanzaBogota.Services.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserApplication _userApplication;

        public UserController(IUserApplication userApplication)
        {
            _userApplication = userApplication;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userApplication.GetAllAsync();
            return Ok(result);
        }

        [HttpPatch("change-role")]
        public async Task<IActionResult> ChangeRole([FromBody] ChangeRoleDto dto)
        {
            var result = await _userApplication.ChangeRoleAsync(dto);

            if (!result)
            {
                return BadRequest(new
                {
                    message = "No se pudo cambiar el rol. Verifica el usuario y el rol indicado."
                });
            }

            return Ok(new { message = "Rol actualizado correctamente." });
        }
    }
}
