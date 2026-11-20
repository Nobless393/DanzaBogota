using DanzaBogota.Application.Dto.EventComment;
using DanzaBogota.Application.Interface;
using DanzaBogota.Transversal.Common.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DanzaBogota.Services.WebApi.Controllers
{
    [ApiController]
    [Route("api/Event/{eventId}/comments")]
    public class EventCommentController : ControllerBase
    {
        private readonly IEventCommentApplication _commentApplication;

        public EventCommentController(IEventCommentApplication commentApplication)
        {
            _commentApplication = commentApplication;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int eventId, [FromQuery] string sort = "recent")
        {
            var (userId, isAdminOrGestor) = ResolveIdentity();
            var result = await _commentApplication.GetByEventAsync(eventId, sort, userId, isAdminOrGestor);
            return Ok(ApiResponse<IEnumerable<EventCommentDto>>.Ok(result));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(int eventId, [FromBody] CreateEventCommentDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var (success, message, comment) = await _commentApplication.CreateAsync(eventId, userId, dto);

            if (!success)
            {
                return BadRequest(ApiResponse<string>.Fail(message));
            }

            return Ok(ApiResponse<EventCommentDto?>.Ok(comment, message));
        }

        [HttpDelete("{commentId}")]
        [Authorize]
        public async Task<IActionResult> Delete(int eventId, int commentId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isAdminOrGestor = User.IsInRole("Admin") || User.IsInRole("Gestor");
            var (success, message) = await _commentApplication.DeleteAsync(eventId, commentId, userId, isAdminOrGestor);

            if (!success)
            {
                return BadRequest(ApiResponse<string>.Fail(message));
            }

            return Ok(ApiResponse<string>.Ok(message));
        }

        [HttpPost("{commentId}/like")]
        [Authorize]
        public async Task<IActionResult> ToggleLike(int eventId, int commentId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var (success, message, likesCount, liked) = await _commentApplication.ToggleLikeAsync(commentId, userId);

            if (!success)
            {
                return BadRequest(ApiResponse<string>.Fail(message));
            }

            return Ok(ApiResponse<object?>.Ok(new { likesCount, liked }, message));
        }

        private (int UserId, bool IsAdminOrGestor) ResolveIdentity()
        {
            var userId = 0;

            if (User.Identity?.IsAuthenticated == true)
            {
                var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (idClaim != null && int.TryParse(idClaim, out var parsed))
                {
                    userId = parsed;
                }
            }

            var isAdminOrGestor = User.IsInRole("Admin") || User.IsInRole("Gestor");
            return (userId, isAdminOrGestor);
        }
    }
}
