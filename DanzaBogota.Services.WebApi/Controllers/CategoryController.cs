using DanzaBogota.Application.Dto.Category;
using DanzaBogota.Application.Interface;
using DanzaBogota.Transversal.Common.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DanzaBogota.Services.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryApplication _categoryApplication;

        public CategoryController(ICategoryApplication categoryApplication)
        {
            _categoryApplication = categoryApplication;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _categoryApplication.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<CategoryDto>>.Ok(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _categoryApplication.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(ApiResponse<string>.Fail("Categoría no encontrada."));
            }

            return Ok(ApiResponse<CategoryDto>.Ok(result));
        }
    }
}
