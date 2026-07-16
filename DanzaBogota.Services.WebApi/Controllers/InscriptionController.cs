using DanzaBogota.Application.Dto.Inscription;
using DanzaBogota.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DanzaBogota.Services.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InscriptionController : ControllerBase
    {
        private readonly IInscriptionApplication _inscriptionApplication;

        public InscriptionController(IInscriptionApplication inscriptionApplication)
        {
            _inscriptionApplication = inscriptionApplication;
        }

        [HttpPost]
        [Authorize(Roles = "Grupo")]
        public async Task<IActionResult> Create([FromBody] CreateInscriptionDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var (success, message) = await _inscriptionApplication.CreateAsync(dto, userId);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }

        [HttpGet("my-inscriptions")]
        [Authorize(Roles = "Grupo")]
        public async Task<IActionResult> GetMyInscriptions()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _inscriptionApplication.GetMyInscriptionsAsync(userId);
            return Ok(result);
        }

        [HttpGet("event/{eventId}")]
        [Authorize(Roles = "Admin,Gestor")]
        public async Task<IActionResult> GetByEvent(int eventId)
        {
            var result = await _inscriptionApplication.GetByEventAsync(eventId);
            return Ok(result);
        }

        [HttpPatch("review")]
        [Authorize(Roles = "Admin,Gestor")]
        public async Task<IActionResult> Review([FromBody] ReviewInscriptionDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var (success, message) = await _inscriptionApplication.ReviewAsync(dto, userId);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }

        [HttpPatch("{id}/cancel")]
        [Authorize(Roles = "Grupo")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var (success, message) = await _inscriptionApplication.CancelAsync(id, userId);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }
    }
}
