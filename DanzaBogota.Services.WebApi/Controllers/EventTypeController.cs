using DanzaBogota.Application.Dto.Event;
using DanzaBogota.Application.Interface.EventType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DanzaBogota.Services.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventTypeController : ControllerBase
    {
        private readonly IEventTypeApplication _eventTypeApplication;

        public EventTypeController(IEventTypeApplication eventTypeApplication)
        {
            _eventTypeApplication = eventTypeApplication;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var result = await _eventTypeApplication.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _eventTypeApplication.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] EventTypeDto dto)
        {
            var result = await _eventTypeApplication.CreateAsync(dto);
            return Ok(new { affected = result });
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] EventTypeDto dto)
        {
            var result = await _eventTypeApplication.UpdateAsync(dto);

            if (result == 0)
            {
                return NotFound();
            }

            return Ok(new { affected = result });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _eventTypeApplication.DeleteAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return Ok(new { message = "Tipo de evento eliminado correctamente." });
        }
    }
}
