using DanzaBogota.Application.Dto.Inscription;
using DanzaBogota.Application.Dto.Participant;

namespace DanzaBogota.Application.Interface
{
    public interface IInscriptionApplication
    {
        // Director inscribe su grupo a un evento
        Task<(bool Success, string Message)> CreateAsync(CreateInscriptionDto dto, int userId);

        // Director ve las inscripciones de sus grupos
        Task<IEnumerable<InscriptionDto>> GetMyInscriptionsAsync(int userId);

        // Gestor/Admin ve todas las inscripciones de un evento
        Task<IEnumerable<InscriptionDto>> GetByEventAsync(int eventId);

        // Gestor/Admin aprueba o rechaza una inscripción
        Task<(bool Success, string Message)> ReviewAsync(ReviewInscriptionDto dto, int userId);

        // Director cancela su inscripción
        Task<(bool Success, string Message)> CancelAsync(int inscriptionId, int userId);

        // Gestor/Admin o el grupo dueño consultan los participantes de una inscripción
        Task<(bool Success, string Message, IEnumerable<ParticipantDto> Data)> GetParticipantsAsync(
            int inscriptionId, int userId, bool isAdminOrGestor);

        // Gestor/Admin o el grupo dueño reemplazan los participantes mientras la inscripción esté pendiente
        Task<(bool Success, string Message)> UpdateParticipantsAsync(
            int inscriptionId, int userId, bool isAdminOrGestor, UpdateInscriptionParticipantsDto dto);

        // Gestor/Admin: estadísticas globales de inscripciones
        Task<InscriptionStatsDto> GetStatsAsync();
    }
}
