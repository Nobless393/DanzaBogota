using DanzaBogota.Application.Dto.Inscription;
using DanzaBogota.Application.Interface;
using DanzaBogota.Domain.Entity.Enums;
using DanzaBogota.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DanzaBogota.Application.Main.Inscription
{
    public class InscriptionApplication : IInscriptionApplication
    {
        private readonly ApplicationDbContext _context;

        public InscriptionApplication(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message)> CreateAsync(CreateInscriptionDto dto, int userId)
        {
            // 1. Verifica que el grupo pertenezca al usuario
            var group = await _context.Groups
                .Include(g => g.Category)
                .Include(g => g.Participants.Where(p => p.IsActive))
                .FirstOrDefaultAsync(g => g.Id == dto.GroupId && g.UserId == userId && g.IsActive);

            if (group == null)
                return (false, "Grupo no encontrado o no tienes permiso.");

            // 2. Verifica que el evento exista y esté publicado
            var eventEntity = await _context.Events
                .Include(e => e.Categories)
                .Include(e => e.Applications)
                .FirstOrDefaultAsync(e => e.Id == dto.EventId && e.IsActive);

            if (eventEntity == null)
                return (false, "Evento no encontrado.");

            if (eventEntity.Status != EventStatus.Published)
                return (false, "El evento no está disponible para inscripciones.");

            // 3. Verifica que la categoría del grupo coincida con las del evento
            var categoryMatch = eventEntity.Categories
                .Any(c => c.Id == group.CategoryId);

            if (!categoryMatch)
                return (false, "La categoría de tu grupo no coincide con las categorías del evento.");

            // 4. Verifica que el grupo no esté ya inscrito
            var alreadyApplied = await _context.EventApplications
                .AnyAsync(a => a.GroupId == dto.GroupId && a.EventId == dto.EventId
                    && a.Status != ApplicationStatus.Cancelled);

            if (alreadyApplied)
                return (false, "Tu grupo ya está inscrito en este evento.");

            // 5. Verifica si el evento requiere RUT
            if (eventEntity.RequiresRut && string.IsNullOrEmpty(group.RutUrl) && !group.UseDirectorRut)
                return (false, "Este evento requiere RUT. Por favor sube el RUT de tu grupo.");

            // 6. Verifica cupos disponibles
            var approvedApplications = eventEntity.Applications
                .Count(a => a.Status == ApplicationStatus.Approved);

            if (approvedApplications >= eventEntity.MaxParticipants)
                return (false, "El evento ha alcanzado el máximo de participantes.");

            // 7. Verifica participantes por grupo
            if (group.Participants.Count > eventEntity.MaxParticipantsPerGroup)
                return (false, $"Tu grupo supera el máximo de {eventEntity.MaxParticipantsPerGroup} participantes por grupo.");

            // 8. Crea la inscripción
            var inscription = new Domain.Entity.Entities.EventApplication
            {
                GroupId = dto.GroupId,
                EventId = dto.EventId,
                AppliedAt = DateTimeOffset.UtcNow,
                Status = ApplicationStatus.Pending,
                Created = DateTimeOffset.UtcNow,
                CreatedBy = userId.ToString()
            };

            await _context.EventApplications.AddAsync(inscription);
            await _context.SaveChangesAsync();

            return (true, "Inscripción realizada correctamente. Pendiente de aprobación.");
        }

        public async Task<IEnumerable<InscriptionDto>> GetMyInscriptionsAsync(int userId)
        {
            return await _context.EventApplications
                .Include(a => a.Group)
                    .ThenInclude(g => g.Category)
                .Include(a => a.Event)
                .Include(a => a.Participants)
                .Where(a => a.Group.UserId == userId)
                .Select(a => new InscriptionDto
                {
                    Id = a.Id,
                    GroupId = a.GroupId,
                    GroupName = a.Group.Name,
                    CategoryName = a.Group.Category.Name,
                    EventId = a.EventId,
                    EventTitle = a.Event.Title,
                    AppliedAt = a.AppliedAt,
                    Status = a.Status.ToString(),
                    Observations = a.Observations,
                    ParticipantCount = a.Participants.Count
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<InscriptionDto>> GetByEventAsync(int eventId)
        {
            return await _context.EventApplications
                .Include(a => a.Group)
                    .ThenInclude(g => g.Category)
                .Include(a => a.Event)
                .Include(a => a.Participants)
                .Where(a => a.EventId == eventId)
                .Select(a => new InscriptionDto
                {
                    Id = a.Id,
                    GroupId = a.GroupId,
                    GroupName = a.Group.Name,
                    CategoryName = a.Group.Category.Name,
                    EventId = a.EventId,
                    EventTitle = a.Event.Title,
                    AppliedAt = a.AppliedAt,
                    Status = a.Status.ToString(),
                    Observations = a.Observations,
                    ParticipantCount = a.Participants.Count
                })
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> ReviewAsync(ReviewInscriptionDto dto, int userId)
        {
            var inscription = await _context.EventApplications
                .Include(a => a.Event)
                .FirstOrDefaultAsync(a => a.Id == dto.InscriptionId
                    && a.Status == ApplicationStatus.Pending);

            if (inscription == null)
                return (false, "Inscripción no encontrada o ya fue revisada.");

            // Verifica que el usuario sea el creador del evento
            var isEventOwner = inscription.Event.CreatedByUserId == userId;
            var isAdmin = await _context.EventApplications
                .AnyAsync(a => a.Id == dto.InscriptionId);

            inscription.Status = dto.Approved
                ? ApplicationStatus.Approved
                : ApplicationStatus.Rejected;

            inscription.Observations = dto.Observations;
            inscription.LastModified = DateTimeOffset.UtcNow;
            inscription.LastModifiedBy = userId.ToString();

            await _context.SaveChangesAsync();

            var statusMsg = dto.Approved ? "aprobada" : "rechazada";
            return (true, $"Inscripción {statusMsg} correctamente.");
        }

        public async Task<(bool Success, string Message)> CancelAsync(int inscriptionId, int userId)
        {
            var inscription = await _context.EventApplications
                .Include(a => a.Group)
                .FirstOrDefaultAsync(a => a.Id == inscriptionId
                    && a.Group.UserId == userId
                    && a.Status == ApplicationStatus.Pending);

            if (inscription == null)
                return (false, "Inscripción no encontrada o no puede cancelarse.");

            inscription.Status = ApplicationStatus.Cancelled;
            inscription.LastModified = DateTimeOffset.UtcNow;
            inscription.LastModifiedBy = userId.ToString();

            await _context.SaveChangesAsync();
            return (true, "Inscripción cancelada correctamente.");
        }
    }
}
