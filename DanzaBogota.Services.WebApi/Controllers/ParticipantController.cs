using DanzaBogota.Application.Dto.Participant;
using DanzaBogota.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DanzaBogota.Services.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Grupo")]
    public class ParticipantController : ControllerBase
    {
        private readonly IParticipantApplication _participantApplication;

        public ParticipantController(IParticipantApplication participantApplication)
        {
            _participantApplication = participantApplication;
        }

        [HttpGet("group/{groupId}")]
        public async Task<IActionResult> GetByGroup(int groupId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _participantApplication.GetByGroupAsync(groupId, userId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _participantApplication.GetByIdAsync(id, userId);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateParticipantDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _participantApplication.CreateAsync(dto, userId);

            if (result == 0)
            {
                return BadRequest(new { message = "No tienes permiso para agregar participantes a este grupo." });
            }

            return Ok(new { affected = result });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateParticipantDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _participantApplication.UpdateAsync(dto, userId);

            if (result == 0)
            {
                return NotFound();
            }

            return Ok(new { affected = result });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _participantApplication.DeleteAsync(id, userId);

            if (!result)
            {
                return NotFound();
            }

            return Ok(new { message = "Participante eliminado correctamente." });
        }
    }
}
