using DanzaBogota.Application.Dto.Inscription;
using DanzaBogota.Application.Dto.Participant;
using DanzaBogota.Application.Interface;
using DanzaBogota.Transversal.Common.Responses;
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
                return BadRequest(ApiResponse<string>.Fail(message));
            }

            return Ok(ApiResponse<string>.Ok(message));
        }

        [HttpGet("my-inscriptions")]
        [Authorize(Roles = "Grupo")]
        public async Task<IActionResult> GetMyInscriptions()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _inscriptionApplication.GetMyInscriptionsAsync(userId);
            return Ok(ApiResponse<IEnumerable<InscriptionDto>>.Ok(result));
        }

        [HttpGet("event/{eventId}")]
        [Authorize(Roles = "Admin,Gestor")]
        public async Task<IActionResult> GetByEvent(int eventId)
        {
            var result = await _inscriptionApplication.GetByEventAsync(eventId);
            return Ok(ApiResponse<IEnumerable<InscriptionDto>>.Ok(result));
        }

        [HttpPatch("review")]
        [Authorize(Roles = "Admin,Gestor")]
        public async Task<IActionResult> Review([FromBody] ReviewInscriptionDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var (success, message) = await _inscriptionApplication.ReviewAsync(dto, userId);

            if (!success)
            {
                return BadRequest(ApiResponse<string>.Fail(message));
            }

            return Ok(ApiResponse<string>.Ok(message));
        }

        [HttpPatch("{id}/cancel")]
        [Authorize(Roles = "Grupo")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var (success, message) = await _inscriptionApplication.CancelAsync(id, userId);

            if (!success)
            {
                return BadRequest(ApiResponse<string>.Fail(message));
            }

            return Ok(ApiResponse<string>.Ok(message));
        }

        [HttpGet("{id}/participants")]
        [Authorize]
        public async Task<IActionResult> GetParticipants(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isAdminOrGestor = User.IsInRole("Admin") || User.IsInRole("Gestor");
            var (success, message, data) = await _inscriptionApplication.GetParticipantsAsync(id, userId, isAdminOrGestor);

            if (!success)
            {
                return NotFound(ApiResponse<string>.Fail(message));
            }

            return Ok(ApiResponse<IEnumerable<ParticipantDto>>.Ok(data));
        }

        [HttpGet("stats")]
        [Authorize(Roles = "Admin,Gestor")]
        public async Task<IActionResult> GetStats()
        {
            var result = await _inscriptionApplication.GetStatsAsync();
            return Ok(ApiResponse<InscriptionStatsDto>.Ok(result));
        }

        [HttpPut("{id}/participants")]
        [Authorize]
        public async Task<IActionResult> UpdateParticipants(int id, [FromBody] UpdateInscriptionParticipantsDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isAdminOrGestor = User.IsInRole("Admin") || User.IsInRole("Gestor");
            var (success, message) = await _inscriptionApplication.UpdateParticipantsAsync(id, userId, isAdminOrGestor, dto);

            if (!success)
            {
                return BadRequest(ApiResponse<string>.Fail(message));
            }

            return Ok(ApiResponse<string>.Ok(message));
        }
    }
}
