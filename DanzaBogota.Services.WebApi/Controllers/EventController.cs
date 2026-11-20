using DanzaBogota.Application.Dto.Event;
using DanzaBogota.Application.Interface;
using DanzaBogota.Transversal.Common.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DanzaBogota.Services.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventApplication _eventApplication;

        public EventController(IEventApplication eventApplication)
        {
            _eventApplication = eventApplication;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Gestor")]
        public async Task<IActionResult> GetAll([FromQuery] EventQueryParameters query)
        {
            var result = await _eventApplication.GetAllAsync(query);
            return Ok(ApiResponse<PaginatedResult<EventDto>>.Ok(result));
        }

        [HttpGet("finished-count")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFinishedCount()
        {
            var result = await _eventApplication.GetFinishedCountAsync();
            return Ok(ApiResponse<int>.Ok(result));
        }

        [HttpGet("published")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublished([FromQuery] EventQueryParameters query)
        {
            var result = await _eventApplication.GetPublishedAsync(query);
            return Ok(ApiResponse<PaginatedResult<EventDto>>.Ok(result));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _eventApplication.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(ApiResponse<string>.Fail("Evento no encontrado."));
            }

            return Ok(ApiResponse<EventDto>.Ok(result));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Gestor")]
        public async Task<IActionResult> Create([FromBody] CreateEventDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _eventApplication.CreateAsync(dto, userId);
            return Ok(ApiResponse<int>.Ok(result, "Evento creado correctamente."));
        }

        [HttpPut]
        [Authorize(Roles = "Admin,Gestor")]
        public async Task<IActionResult> Update([FromBody] UpdateEventDto dto)
        {
            var result = await _eventApplication.UpdateAsync(dto);

            if (result == 0)
            {
                return NotFound(ApiResponse<string>.Fail("Evento no encontrado."));
            }

            return Ok(ApiResponse<int>.Ok(result, "Evento actualizado correctamente."));
        }

        [HttpPatch("{id}/publish")]
        [Authorize(Roles = "Admin,Gestor")]
        public async Task<IActionResult> Publish(int id)
        {
            var result = await _eventApplication.PublishAsync(id);

            if (!result)
            {
                return BadRequest(ApiResponse<string>.Fail("No se puede publicar el evento."));
            }

            return Ok(ApiResponse<string>.Ok("Evento publicado correctamente."));
        }

        [HttpPatch("{id}/cancel")]
        [Authorize(Roles = "Admin,Gestor")]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _eventApplication.CancelAsync(id);

            if (!result)
            {
                return BadRequest(ApiResponse<string>.Fail("No se puede cancelar el evento."));
            }

            return Ok(ApiResponse<string>.Ok("Evento cancelado correctamente."));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _eventApplication.DeleteAsync(id);

            if (!result)
            {
                return NotFound(ApiResponse<string>.Fail("Evento no encontrado."));
            }

            return Ok(ApiResponse<string>.Ok("Evento eliminado correctamente."));
        }
    }
}
