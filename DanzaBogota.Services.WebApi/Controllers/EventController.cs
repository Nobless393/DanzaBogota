using DanzaBogota.Application.Dto.Event;
using DanzaBogota.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> GetAll()
        {
            var result = await _eventApplication.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("published")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublished()
        {
            var result = await _eventApplication.GetPublishedAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _eventApplication.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Gestor")]
        public async Task<IActionResult> Create([FromBody] CreateEventDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _eventApplication.CreateAsync(dto, userId);
            return Ok(new { affected = result });
        }

        [HttpPut]
        [Authorize(Roles = "Admin,Gestor")]
        public async Task<IActionResult> Update([FromBody] UpdateEventDto dto)
        {
            var result = await _eventApplication.UpdateAsync(dto);

            if (result == 0)
            {
                return NotFound();
            }

            return Ok(new { affected = result });
        }

        [HttpPatch("{id}/publish")]
        [Authorize(Roles = "Admin,Gestor")]
        public async Task<IActionResult> Publish(int id)
        {
            var result = await _eventApplication.PublishAsync(id);

            if (!result)
            {
                return BadRequest("No se puede publicar el evento.");
            }

            return Ok(new { message = "Evento publicado correctamente." });
        }

        [HttpPatch("{id}/cancel")]
        [Authorize(Roles = "Admin,Gestor")]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _eventApplication.CancelAsync(id);

            if (!result)
            {
                return BadRequest("No se puede cancelar el evento.");
            }

            return Ok(new { message = "Evento cancelado correctamente." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _eventApplication.DeleteAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return Ok(new { message = "Evento eliminado correctamente." });
        }
    }
}
