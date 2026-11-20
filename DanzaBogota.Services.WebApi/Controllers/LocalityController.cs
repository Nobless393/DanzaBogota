using DanzaBogota.Application.Dto.Locality;
using DanzaBogota.Application.Interface;
using DanzaBogota.Transversal.Common.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DanzaBogota.Services.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LocalityController : ControllerBase
    {
        private readonly ILocalityApplication _localityApplication;

        public LocalityController(ILocalityApplication localityApplication)
        {
            _localityApplication = localityApplication;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _localityApplication.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<LocalityDto>>.Ok(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _localityApplication.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(ApiResponse<string>.Fail("Localidad no encontrada."));
            }

            return Ok(ApiResponse<LocalityDto>.Ok(result));
        }
    }
}
