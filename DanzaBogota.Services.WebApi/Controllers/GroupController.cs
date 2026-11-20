using DanzaBogota.Application.Dto.Group;
using DanzaBogota.Application.Interface;
using DanzaBogota.Transversal.Common.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DanzaBogota.Services.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupApplication _groupApplication;

        public GroupController(IGroupApplication groupApplication)
        {
            _groupApplication = groupApplication;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _groupApplication.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<GroupDto>>.Ok(result));
        }

        [HttpGet("my-groups")]
        [Authorize(Roles = "Grupo")]
        public async Task<IActionResult> GetMyGroups()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _groupApplication.GetMyGroupsAsync(userId);
            return Ok(ApiResponse<IEnumerable<GroupDto>>.Ok(result));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _groupApplication.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(ApiResponse<string>.Fail("Grupo no encontrado."));
            }

            return Ok(ApiResponse<GroupDto>.Ok(result));
        }

        [HttpPost]
        [Authorize(Roles = "Grupo")]
        public async Task<IActionResult> Create([FromBody] CreateGroupDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _groupApplication.CreateAsync(dto, userId);
            return Ok(ApiResponse<int>.Ok(result, "Grupo creado correctamente."));
        }

        [HttpPut]
        [Authorize(Roles = "Grupo,Admin")]
        public async Task<IActionResult> Update([FromBody] UpdateGroupDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _groupApplication.UpdateAsync(dto, userId);

            if (result == 0)
            {
                return NotFound(ApiResponse<string>.Fail("Grupo no encontrado."));
            }

            return Ok(ApiResponse<int>.Ok(result, "Grupo actualizado correctamente."));
        }

        [HttpPost("{id}/upload-rut")]
        [Authorize(Roles = "Grupo")]
        public async Task<IActionResult> UploadRut(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<string>.Fail("Debes adjuntar un archivo."));
            }

            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(ApiResponse<string>.Fail("Solo se permiten archivos PDF, JPG o PNG."));
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            using var stream = file.OpenReadStream();
            var result = await _groupApplication.UploadRutAsync(id, userId, stream, file.FileName);

            if (!result)
            {
                return NotFound(ApiResponse<string>.Fail("Grupo no encontrado o no tienes permiso."));
            }

            return Ok(ApiResponse<string>.Ok("RUT subido correctamente."));
        }

        [HttpPost("{id}/upload-photo")]
        [Authorize(Roles = "Grupo")]
        public async Task<IActionResult> UploadPhoto(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<string>.Fail("Debes adjuntar una imagen."));
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(ApiResponse<string>.Fail("Solo se permiten imágenes JPG, PNG o WEBP."));
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            using var stream = file.OpenReadStream();
            var result = await _groupApplication.UploadPhotoAsync(id, userId, stream, file.FileName);

            if (!result)
            {
                return NotFound(ApiResponse<string>.Fail("Grupo no encontrado o no tienes permiso."));
            }

            return Ok(ApiResponse<string>.Ok("Foto subida correctamente."));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Grupo,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _groupApplication.DeleteAsync(id, userId);

            if (!result)
            {
                return NotFound(ApiResponse<string>.Fail("Grupo no encontrado."));
            }

            return Ok(ApiResponse<string>.Ok("Grupo eliminado correctamente."));
        }
    }
}
