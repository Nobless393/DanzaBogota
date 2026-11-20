using DanzaBogota.Application.Dto.EventComment;

namespace DanzaBogota.Application.Interface
{
    public interface IEventCommentApplication
    {
        // Comentarios y respuestas de un evento, ordenados por recencia o popularidad
        Task<IEnumerable<EventCommentDto>> GetByEventAsync(int eventId, string sort, int userId, bool isAdminOrGestor);

        // Cualquier usuario autenticado comenta o responde
        Task<(bool Success, string Message, EventCommentDto? Comment)> CreateAsync(
            int eventId, int userId, CreateEventCommentDto dto);

        // Autor del comentario, organizador del evento o Admin/Gestor pueden eliminar
        Task<(bool Success, string Message)> DeleteAsync(
            int eventId, int commentId, int userId, bool isAdminOrGestor);

        // Reacción tipo "me gusta" con efecto toggle
        Task<(bool Success, string Message, int LikesCount, bool Liked)> ToggleLikeAsync(int commentId, int userId);
    }
}
